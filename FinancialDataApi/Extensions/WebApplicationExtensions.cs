using FinancialDataApi.Endpoints;
using FinancialDataApi.Hubs;
using FinancialDataApi.Models;

namespace FinancialDataApi.Extensions;

public static class WebApplicationExtensions
{
    public static void AddEndpoints(
        this WebApplication app
    )
    {
        // Auth enpoints
        app.MapPost("/auth/register", AuthEndpoints.Register)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<RegistrationResponse>(StatusCodes.Status201Created);

        app.MapPost("/auth/login", AuthEndpoints.Login)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<AuthenticationResponse>(StatusCodes.Status200OK);

        // News endpoints
        app.MapGet("/news/get-all", NewsFeedEndpoints.GetAll)
            .Produces<List<NewsDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();

        app.MapGet("/news/get-by-date-interval", NewsFeedEndpoints.GetByDateInterval)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<List<NewsDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();

        app.MapGet("/news/get-by-key-word", NewsFeedEndpoints.GetByKeyWord)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<List<NewsDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();

        app.MapGet("/news/get-by-instrument", NewsFeedEndpoints.GetByInstrument)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<List<NewsDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();

        app.MapGet("/news/get-latest", NewsFeedEndpoints.GetLatest)
            .Produces<List<NewsDto>>(StatusCodes.Status200OK);

        // News ws enpoint
        app.MapHub<NewsHub>("/news-feed")
            .RequireAuthorization();
    }
}
