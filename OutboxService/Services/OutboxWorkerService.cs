using Dapper;
using DbContext;
using Events;
using MassTransit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OutboxService.Constants;
using OutboxService.Models;
using OutboxService.Options;
using System.Text;

namespace OutboxService.Services;

public class OutboxWorkerService
    : BackgroundService
{
    private readonly ILogger<OutboxWorkerService> _logger;
    private readonly OutboxOptions _options;
    private readonly IDBContext _context;
    private readonly IBusControl _busControl;

    private static readonly UnicodeEncoding Encoding = new();

    public OutboxWorkerService(
        ILogger<OutboxWorkerService> logger,
        OutboxOptions options,
        IDBContext context,
        IBusControl busControl
    )
    {
        _logger = logger;
        _options = options;
        _context = context;
        _busControl = busControl;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        _logger.LogInformation("Outbox started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connection = _context.GetConnection();
                var outboxEntries = await connection.QueryAsync<OutboxEntry>(
                    new(
                        string.Format(Queries.GetOutboxRecords, _options.BatchSize),
                        cancellationToken: stoppingToken
                    )
                );

                var events = outboxEntries
                    .Select(entry => JsonConvert.DeserializeObject<NewsUpdatedEvent>(Encoding.GetString(entry.Data)))
                    .ToArray();

                if (events.Length == 0)
                {
                    await Task.Delay(_options.DelayInMs, stoppingToken);
                    continue;
                }

                await _busControl.PublishBatch(
                    events,
                    stoppingToken
                );

                await connection.ExecuteAsync(
                    new(
                        Queries.DeleteOutboxRecords,
                        new { ids = outboxEntries.Select(entry => entry.Id).ToArray() },
                        cancellationToken: stoppingToken
                    )
                );

                await Task.Delay(_options.DelayInMs, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception was caught while outboxing entries.");
            }
        }

        _logger.LogInformation("Outbox shutting down at: {time}", DateTimeOffset.Now);
    }
}