using FinancialDataApi.Data;
using FinancialDataApi.Endpoints;
using FinancialDataApi.Extensions;
using FinancialDataApi.Hubs;
using FinancialDataApi.Models;
using FinancialDataApi.Options;
using Infrastructure.Extensions;
using MassTransit.SignalR;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

var apiOptions = builder.Configuration
    .GetSection(typeof(FinancialDataApiOptions).Name)
    .Get<FinancialDataApiOptions>();

builder.Services
    .AddSingleton(apiOptions)
    // Since here we have consumers and we are using outbox (meaning we guarantee events are delivered AT LEAST once (maybe more))
    // event idempotency would be good idea. Possible implementation could be using MT filters https://masstransit.io/documentation/configuration/middleware#filters
    // (store event hashes in file/db and load in memory on start (concurrent dictionary?) and check if current consumed event has been already processed)
    .AddServiceBus(apiOptions.RabbitMqOptions, apiOptions.EncryptKey, true, bus => bus.AddSignalRHub<NewsHub>())
    .AddOTELTracing(apiOptions.TracingOptions, addSqlServerInstrumentarium: true)
    .AddDbContext<ApiDatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddAuthServices(apiOptions)
    .AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "test v1"));
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

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

app.Run();