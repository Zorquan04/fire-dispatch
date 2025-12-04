namespace FireDispatch.Models;

// Reprezentuje zgłoszenie/zdarzenie wpływające do SKKM
public class Event(EventType type, Location location)
{
    private static int _counter;

    private int Id { get; } = Interlocked.Increment(ref _counter);
    public EventType Type { get; } = type;
    public Location Location { get; } = location;
    
    public string Label => $"{Type}-{Id}";
}