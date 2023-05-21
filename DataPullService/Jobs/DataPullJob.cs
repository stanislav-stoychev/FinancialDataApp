using DataPullService.Options;
using DbContext;
using Events;
using Events.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace DataPullService.Jobs;

public class DataPullJob
    : IJob
{
    private readonly ILogger<DataPullJob> _logger;
    private readonly DataPullServiceOptions _options;
    private readonly IDBContext _context;
    private readonly HttpClient _httpClient;

    public DataPullJob(
        ILogger<DataPullJob> logger,
        HttpClient httpClient,
        DataPullServiceOptions options,
        IDBContext context
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _options = options;
        _context = context;
    }

    public async Task Execute(
        IJobExecutionContext context
    )
    {
        try
        {
            // Due to API requests limit (using free plan) "enriching" with ohlc data will be faked
            var news = await GetNews();
            List<Content> results = null;
            try
            {
                results = JsonConvert.DeserializeObject<List<Content>>(news.SelectToken("results").ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception was caught while deserializing news data.");
            }

            if (!results?.Any() ?? false)
                return;

            // get distinct tickers
            var tickers = results
                .SelectMany(t => t.tickers)
                .Distinct()
                .ToArray();

            // split to batches
            var groups = tickers
                .Select((ticker, index) => new { ticker, index })
                .GroupBy(x => x.index / _options.BatchSize)
                .Select(g => g.Select(x => x.ticker).ToArray())
                .ToArray();

            // run tasks to get candlestick data
            var tickerTasks = new List<Task<List<Ticker>>>();
            foreach (var grp in groups)
                tickerTasks.Add(GetCandlestickData(grp));

            // wait for all tasks to complete
            var tickersCandlesticks = (await Task.WhenAll(tickerTasks))
                .SelectMany(c => c)
                .ToList();

            var evts = results.Select(r => new NewsUpdatedEvent
            {
                Content = r,
                Tickers = tickersCandlesticks.Where(t => r.tickers.Contains(t.TickerCode)).ToList()
            }).ToArray();

            // persist in db
            await _context.SaveNewsData(
                evts,
                context.CancellationToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception was caught while updating ticker data.");
        }
    }

    private static async Task<List<Ticker>> GetCandlestickData(
        IEnumerable<string> tickers
    )
    {
        await Task.Delay(10);

        var result = new List<Ticker>();
        // convinient number of candles to get, since pull interval is 1 hour, I know it can be computed
        // but I guess this is not the main foucs of the task
        var numberOfCandles = 6;

        foreach (var ticker in tickers)
        {
            // Some fancy logic to get ohlc data for a ticker
            var rng = new Random();

            decimal low = (decimal)(rng.NextDouble() + 0.1);
            decimal open = low + (decimal)(rng.NextDouble());
            decimal close = open + (decimal)(rng.NextDouble());
            decimal high = close + (decimal)(rng.NextDouble());

            var candles = new List<CandleStick>();
            for (int i = 0; i < numberOfCandles; i++)
                candles.Add(new()
                {
                    o = open,
                    h = high,
                    l = low,
                    c = close,
                    t = DateTime.UtcNow.AddMinutes(numberOfCandles * -10)
                });

            candles.Reverse();
            result.Add(new() { TickerCode = ticker, CandleSticks = candles });
        }

        return result;
    }

    private async Task<JObject> GetNews()
    {
        var newsFrom = DateTime.UtcNow.AddHours(-1 * _options.LookbackIntervalInHours)
            .ToString("yyyy-MM-ddThh:mm:ss");
        var resp = await _httpClient.GetAsync($"v2/reference/news?published_utc.gte={newsFrom}");

        return JObject.Parse(await resp.Content.ReadAsStringAsync());
    }
}
