using AutoMapper;
using FinancialDataApi.Data;
using FinancialDataApi.Data.Models;
using FinancialDataApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FinancialDataApi.Endpoints;

public static class NewsFeedEndpoints
{
    public static async Task<Ok<List<NewsDto>>> GetAll(
        ApiDatabaseContext dbContext,
        IMapper mapper,
        CancellationToken token
    )
    {
        var news = await dbContext.News
            .Include(x => x.NewsTickerDatas)
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(mapper.Map<List<News>, List<NewsDto>>(news));
    }

    public static async Task<IResult> GetByDateInterval(
        int lookBackPeriod, 
        ApiDatabaseContext dbContext,
        IMapper mapper,
        CancellationToken token
    )
    {
        if (lookBackPeriod <= 0)
            return TypedResults.BadRequest("Invalid lookback period.");

        var news = await dbContext.News
            .Include(x => x.NewsTickerDatas)
            .Where(x => DateTime.UtcNow.Date.AddDays(-1 * lookBackPeriod) >= x.Date.Date)
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(mapper.Map<List<News>, List<NewsDto>>(news));
    }

    public static async Task<IResult> GetByKeyWord(
        string keyWord,
        ApiDatabaseContext dbContext,
        IMapper mapper,
        CancellationToken token
    )
    {
        if (string.IsNullOrWhiteSpace(keyWord))
            return TypedResults.BadRequest("Search key must not be empty.");

        var news = await dbContext.News
            .Include(x => x.NewsTickerDatas)
            .Where(x => x.Description.Contains(keyWord))
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(mapper.Map<List<News>, List<NewsDto>>(news));
    }

    public static async Task<IResult> GetByInstrument(
        string instrument,
        ApiDatabaseContext dbContext,
        IMapper mapper,
        CancellationToken token,
        int limit = 10
    )
    {
        if (string.IsNullOrWhiteSpace(instrument))
            return TypedResults.BadRequest("Insrument must not be empty.");

        if(limit <= 0)
            return TypedResults.BadRequest("Limit must be number greater than zero.");

        var news = await dbContext.NewsTickerData
            .Where(n => n.Ticker.Equals(instrument))
            .Include(n => n.News)
            .OrderByDescending(n => n.News.Date)
            .Take(limit)
            .Select(n => n.News)
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(mapper.Map<List<News>, List<NewsDto>>(news));
    }

    public static async Task<Ok<List<NewsDto>>> GetLatest(
        ApiDatabaseContext dbContext,
        IMapper mapper,
        CancellationToken token,
        int top = 5
    )
    {
        var news = await dbContext.News
            .Include(x => x.NewsTickerDatas)
            .OrderByDescending(x => x.Date)
            .Take(top)
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(mapper.Map<List<News>, List<NewsDto>>(news));
    }
}