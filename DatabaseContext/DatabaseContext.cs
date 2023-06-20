using Dapper;
using Events;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace DbContext;

public interface IDBContext
{
    Task SaveNewsData(
        NewsUpdatedEvent[] evts,
        CancellationToken token = default
    );

    IDbConnection GetConnection(
        bool withDbName = true
    );
}

public class DatabaseContext
    : IDBContext
{
    private readonly string _dataSource;
    private readonly object _lock = new();
    private const string SchemaDefinitionFileName = "CreateSchema.sql";
    private bool _isDbInitialised = false;

    public DatabaseContext(
        string dataSource,
        bool initDatabase = false
    )
    {
        _isDbInitialised = !initDatabase;
        _dataSource = dataSource;

        EnsureDbCreated();
    }

    public async Task SaveNewsData(
        NewsUpdatedEvent[] evts,
        CancellationToken token = default
    )
    {
        using var connection = GetConnection();
        await connection.ExecuteAsync(
            new(
                "[dbo].[SaveNewsData]",
                new
                {
                    events = JsonConvert.SerializeObject(evts, Formatting.None)
                },
                cancellationToken: token,
                commandType: CommandType.StoredProcedure
            )
        );
    }

    public IDbConnection GetConnection(
        bool withDbName = true
    ) => new SqlConnection(string.Concat(_dataSource, withDbName ? "Database=FinancialData;" : string.Empty));

    private static string ReadResource(
        string name
    )
    {
        var assembly = Assembly.GetExecutingAssembly();

        string resourcePath = assembly.GetManifestResourceNames()
                                       .Single(str => str.EndsWith(name));

        using var stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private void EnsureDbCreated()
    {
        lock (_lock)
        {
            if (_isDbInitialised)
                return;

            string command = ReadResource(SchemaDefinitionFileName);
            var cmds = command.Replace("GO", "~")
                                   .Split('~')
                                   .Where(cmd => !string.IsNullOrEmpty(cmd));

            var enumerator = cmds.GetEnumerator();
            enumerator.MoveNext();
            string createDb = enumerator.Current;

            using var connection = GetConnection(false);
            connection.Execute(createDb);

            using var connectionWithDb = GetConnection();
            while (enumerator.MoveNext())
                connectionWithDb.Execute(enumerator.Current);

            _isDbInitialised = true;
        }
    }
}
