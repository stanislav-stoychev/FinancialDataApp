namespace OutboxService.Constants;

public static class Queries
{
    public static string GetOutboxRecords = @"
    SELECT 
        TOP {0} *
    FROM
        [dbo].[Outbox]
    ORDER BY
        [Date]";

    public static string DeleteOutboxRecords = @"
    DELETE FROM
        [dbo].[Outbox]
    WHERE 
        [Id] IN @ids";
}
