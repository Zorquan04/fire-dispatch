using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

public class CommandCenter : ISubject
{
    private readonly List<IObserver> _observers = new();

    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);

    public void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update(message);
    }

    public void NewEvent(Event evt)
    {
        Console.WriteLine($"[SKKM] Nowe zgłoszenie: {evt.Type} na lokalizacji [{evt.Location.Latitude}, {evt.Location.Longitude}]");
        Notify($"Nowe zdarzenie: {evt.Type}");
    }
}