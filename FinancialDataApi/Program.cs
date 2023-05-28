using FinancialDataApi.Data;
using FinancialDataApi.Extensions;
using FinancialDataApi.Hubs;
using FinancialDataApi.Options;
using Infrastructure.Extensions;
using MassTransit.SignalR;
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

app.AddEndpoints();

app.Run();