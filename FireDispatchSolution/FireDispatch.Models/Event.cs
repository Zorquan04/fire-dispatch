namespace FireDispatch.Models;

/// Reprezentuje zgłoszenie/zdarzenie wpływające do SKKM
public class Event(EventType type, Location location)
{
    private Guid Id { get; } = Guid.NewGuid();
    public EventType Type { get; set; } = type;
    public Location Location { get; set; } = location;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public override string ToString()
    {
        return $"Event {{Id={Id}, Type={Type}, Loc=({Location.Latitude},{Location.Longitude})}}";
    }
}