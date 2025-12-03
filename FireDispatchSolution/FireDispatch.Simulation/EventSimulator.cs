using FireDispatch.Context;
using FireDispatch.Models;
using FireDispatch.Interfaces;

namespace FireDispatch.Simulation;

// Symulator zdarzeń PSP — iteruje po zdarzeniach, dysponuje jednostki, powiadamia obserwatorów
public class EventSimulator(DispatchContext dispatcher)
{
    private readonly List<IObserver> _observers = new();

    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);
    private void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update(message);
    }
    
    // Obsługuje pojedyncze zdarzenie
    public async Task HandleEventAsync(Event evt)
    {
        Notify($"Nowe zdarzenie: {evt.Type} w [{evt.Location.Latitude}, {evt.Location.Longitude}]");

        // wybieramy pojazdy za pomocą dispatchera
        var vehicles = dispatcher.Dispatch(evt, evt.Type == EventType.Pz ? 3 : 2);

        if (!vehicles.Any())
        {
            Notify("Brak wolnych pojazdów!");
            return;
        }

        foreach (var v in vehicles)
        {
            v.Assign();
            Notify($"Pojazd {v.Name} wysłany do zdarzenia {evt.Type}");
        }

        // symulacja czasu dojazdu i powrotu (asynchronicznie)
        await Task.Delay(1000); // np. 1s = czas “dojazdu”
        foreach (var v in vehicles)
        {
            v.Free();
            Notify($"Pojazd {v.Name} wrócił do jednostki");
        }
    }
}