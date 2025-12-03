using FireDispatch.Context;
using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;

namespace FireDispatch.Simulation;

public class EventSimulator(DispatchContext dispatcher)
{
    private readonly List<IObserver> _observers = [];
    private readonly Random _rng = new();

    // Statystyki
    private int _totalEvents;
    private int _totalVehiclesDispatched;
    private int _totalTimeMs;

    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);

    private void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update(message);
    }

    public async Task HandleEventAsync(Event evt)
    {
        _totalEvents++;
        Notify($"Nowe zdarzenie: {evt.Type} w [{evt.Location.Latitude:F5}, {evt.Location.Longitude:F5}]");

        var vehicles = dispatcher.Dispatch(evt, evt.Type == EventType.Pz ? 3 : 2);
        if (!vehicles.Any())
        {
            Notify("Brak wolnych pojazdów!");
            return;
        }

        _totalVehiclesDispatched += vehicles.Count;

        var vehicleCollection = new VehicleCollection(vehicles);
        var vehicleIterator = vehicleCollection.GetIterator();

        while (vehicleIterator.HasNext())
        {
            var v = vehicleIterator.Next();
            v.Assign();
            Notify($"Pojazd {v.Name} wysłany do zdarzenia {evt.Type}");
        }

        int travelTime = _rng.Next(0, 3000);
        await Task.Delay(travelTime);
        _totalTimeMs += travelTime;
        Notify($"Pojazdy dotarły na miejsce po {travelTime} ms");

        bool falseAlarm = _rng.NextDouble() < 0.05;
        if (falseAlarm)
        {
            Notify("Alarm fałszywy! Pojazdy wracają...");
            int returnTime = _rng.Next(0, 3000);
            await Task.Delay(returnTime);
            _totalTimeMs += returnTime;
        }
        else
        {
            int actionTime = _rng.Next(5000, 25001);
            Notify($"Akcja trwa {actionTime / 1000.0:F1} s");
            await Task.Delay(actionTime);
            _totalTimeMs += actionTime;

            int returnTime = _rng.Next(0, 3000);
            await Task.Delay(returnTime);
            _totalTimeMs += returnTime;
            Notify($"Pojazdy wróciły do jednostki po {returnTime} ms");
        }

        vehicleIterator = vehicleCollection.GetIterator();
        while (vehicleIterator.HasNext())
        {
            var v = vehicleIterator.Next();
            v.Free();
            Notify($"Pojazd {v.Name} dostępny");
        }
    }

    public void PrintStatistics()
    {
        Console.WriteLine("\n--- STATYSTYKI SYMULACJI ---");
        Console.WriteLine($"Liczba zdarzeń obsłużonych: {_totalEvents}");
        Console.WriteLine($"Łączna liczba wyjeżdżających pojazdów: {_totalVehiclesDispatched}");
        Console.WriteLine($"Łączny czas (ms) akcji i dojazdów: {_totalTimeMs}");
        Console.WriteLine($"Średni czas na zdarzenie (ms): {(_totalEvents > 0 ? _totalTimeMs / _totalEvents : 0)}");
        Console.WriteLine("----------------------------\n");
    }
}