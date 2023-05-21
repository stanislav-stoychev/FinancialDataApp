namespace Events;

public class Event
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
}