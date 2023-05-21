using AutoMapper;
using Events.Models;
using FinancialDataApi.Data.Models;
using FinancialDataApi.Models;
using Newtonsoft.Json;

namespace FinancialDataApi.Profiles;

public class NewsProfile
    : Profile
{
    public NewsProfile()
    {
        CreateMap<News, NewsDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Content>(src.Contents)))
            .ForMember(dest => dest.Tickers, opt => opt.MapFrom(src => ParseTickers(src.NewsTickerDatas)));
    }

    private static List<TickerDto> ParseTickers(
        ICollection<NewsTickerData> tickers
    )
    {
        if (tickers is null)
            return null;

        var result = new List<TickerDto>();
        foreach (var item in tickers)
            result.Add(new()
            {
                Ticker = item.Ticker,
                CandleSticks = JsonConvert.DeserializeObject<List<CandleStick>>(item.CandleStickChart)
            });

        return result;
    }
}