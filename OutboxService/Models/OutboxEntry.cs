namespace OutboxService.Models;

public class OutboxEntry
{
    public long Id { get; set; }
    
    public byte[] Data { get; set; }

    public DateTimeOffset Date { get; set; }
}
