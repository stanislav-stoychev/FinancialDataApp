namespace OutboxService.Constants;

public static class Queries
{
    public const string GetOutboxRecords = @"
    SELECT 
        TOP {0} *
    FROM
        [dbo].[Outbox]
    ORDER BY
        [Date]";

    public const string DeleteOutboxRecords = @"
    DELETE FROM
        [dbo].[Outbox]
    WHERE 
        [Id] IN @ids";
}
