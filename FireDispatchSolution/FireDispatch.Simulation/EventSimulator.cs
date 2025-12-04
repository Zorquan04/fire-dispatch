using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;
using FireDispatch.Strategy;
using System.Collections.Concurrent;

namespace FireDispatch.Simulation;

public class EventSimulator(Dispatcher dispatcher) : IObserver
{
    private readonly List<IObserver> _observers = new();
    private readonly ConcurrentQueue<Event> _pendingEvents = new();

    private int _totalEvents;
    private int _totalVehiclesDispatched;
    private int _totalTimeMs;

    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);

    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        foreach (var obs in _observers.ToArray())
            obs.Update(message, vehicle, state);
    }
    
    public async Task HandleEventAsync(Event evt)
    {
        var rand = new Random();
        await Task.Delay(rand.Next(200, 500)); // małe opóźnienie startu
        var eventStartTime = DateTime.Now;
        Interlocked.Increment(ref _totalEvents);

        Update($"--- NOWE ZDARZENIE: {evt.Label} ---");
        Update($"Lokalizacja: {evt.Location.Latitude:F5}, {evt.Location.Longitude:F5}");

        int requiredCount = evt.Type switch
        {
            EventType.Pz => 3,
            EventType.Mz => 2,
            EventType.Af => 0,
            _ => 2
        };

        if (requiredCount == 0)
        {
            Update("Zgłoszenie oznaczone jako FAŁSZYWE – brak wysyłania pojazdów.");
            return;
        }

        var vehicles = dispatcher.Dispatch(evt, requiredCount).ToList();

        if (!vehicles.Any())
        {
            Update("Brak wolnych pojazdów – dodaję zdarzenie do kolejki oczekujących");
            _pendingEvents.Enqueue(evt);
            return;
        }

        Interlocked.Add(ref _totalVehiclesDispatched, vehicles.Count);
        var vehicleCollection = new VehicleCollection(vehicles);

        var iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Assign();
            Update($"Pojazd {v.Name} przypisany do zdarzenia {evt.Label}", v, v.State);
        }

        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.StartTravel();
            Update($"Pojazd {v.Name} w drodze do zdarzenia {evt.Label}", v, v.State);
            
            int travelTimeMs = rand.Next(1000, 4000);
            Update($"Czas dojazdu pojazdu {v.Name}: {travelTimeMs / 1000.0:F1}s");
            await Task.Delay(travelTimeMs);
        }

        // sprawdzamy czy zdarzenie okaże się fałszywe (5% szansy)
        bool falseAlarm = rand.Next(100) < 50;
        
        // Akcja na miejscu
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Arrive();
            Update($"Pojazd {v.Name} na miejscu zdarzenia {evt.Label}", v, v.State);
    
            // jeśli fałszywy -> 0s akcji i wracają
            if (falseAlarm)
            {
                Update($"FAŁSZYWY ALARM przy {evt.Label}! Jednostki zawracają.", v, v.State);

                v.Return();
                Update($"Pojazd {v.Name} wraca do jednostki (fałszywy alarm)", v, v.State);
                
                continue;   // lecimy dalej, nie robimy akcji
            }

            // normalne zdarzenie
            int actionTimeMs = rand.Next(5000, 15000);
            Update($"Czas aktywności pojazdu {v.Name}: {actionTimeMs / 1000.0:F1}s");
            await Task.Delay(actionTimeMs);
        }

        // Powrót
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            
            if (!falseAlarm)
            {
                v.Return();
                Update($"Pojazd {v.Name} wraca do jednostki", v, v.State);
            }
            
            int returnTimeMs = rand.Next(1000, 4000);
            Update($"Czas powrotu pojazdu {v.Name}: {returnTimeMs / 1000.0:F1}s");
            await Task.Delay(returnTimeMs);
        }

        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Free();
            Update($"Pojazd {v.Name} dostępny ponownie", v, v.State);
        }

        _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;

        await CheckPendingEventsAsync();
    }

    private Task CheckPendingEventsAsync()
    {
        if (_pendingEvents.TryDequeue(out var nextEvent))
            _ = Task.Run(() => HandleEventAsync(nextEvent));

        return Task.CompletedTask;
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