namespace FireDispatch.Models;

// Model of a single incoming report
// Contains an event type (e.g., fire/local threat) and location
public class Event(EventType type, Location location)
{
    // Global event counter – each new report gets a consecutive number
    private static int _counter;

    // Unique event ID automatically assigned in an atomic manner (thread-safe)
    private int Id { get; } = Interlocked.Increment(ref _counter);

    // Event Type (Enum)
    public EventType Type { get; } = type;

    // Place of occurrence
    public Location Location { get; } = location;

    // Auxiliary label e.g. "Pz-4" - good for logging/event identification
    public string Label => $"{Type}-{Id}";
}