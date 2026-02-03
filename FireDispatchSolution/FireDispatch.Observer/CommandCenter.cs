using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// The Command Center (SKKM) serves as the Subject in the Observer pattern.
// Receives new reports and distributes information to all observers (loggers, units, etc.)
public class CommandCenter : ISubject
{
    // List of connected observers (e.g. logger, UnitObserver)
    private readonly List<IObserver> _observers = new();

    // Adding a follower
    public void Attach(IObserver observer) => _observers.Add(observer);

    // Removing a follower
    public void Detach(IObserver observer) => _observers.Remove(observer);

    // Sending a message to all followers
    public void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update("[SKKM] " + message);
    }

    // Triggered when a new event occurs in the system
    public void NewEvent(Event evt)
    {
        // Information on the console about the acceptance of the application
        Console.WriteLine($"[SKKM] New report: {evt.Label} | {evt.Type} | ({evt.Location.Latitude:F5}, {evt.Location.Longitude:F5})");

        // Distribution of information to observers
        Notify($"Report accepted: {evt.Label}");
    }
}