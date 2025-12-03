using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;
using System.Collections.Concurrent;
using FireDispatch.Strategy;

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

    public void Update(string message, VehicleState? state = null)
    {
        foreach (var obs in _observers.ToArray())
        {
            obs.Update(message, state);
        }

        if (state != null)
        {
            Console.ForegroundColor = state switch
            {
                VehicleState.Free => ConsoleColor.Green,
                VehicleState.Assigned => ConsoleColor.Yellow,
                VehicleState.EnRoute => ConsoleColor.Cyan,
                VehicleState.OnScene => ConsoleColor.Magenta,
                VehicleState.Returning => ConsoleColor.DarkCyan,
                _ => ConsoleColor.White
            };
        }
        else Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine(message);
        Console.ResetColor();
    }

    public async Task HandleEventAsync(Event evt)
    {
        var rand = new Random();
        var eventStartTime = DateTime.Now;
        _totalEvents++;

        Update($"--- NOWE ZDARZENIE: {evt.Type} ---");
        Update($"Lokalizacja: {evt.Location.Latitude}, {evt.Location.Longitude}");

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

        _totalVehiclesDispatched += vehicles.Count;
        var vehicleCollection = new VehicleCollection(vehicles);

        // Iteratory zamiast foreach
        var iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Assign();
            Update($"Pojazd {v.Name} przypisany do zdarzenia {evt.Type}", v.State);
        }

        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.StartTravel();
            Update($"Pojazd {v.Name} w drodze", v.State);
        }

        int travelTimeMs = rand.Next(1000, 4000);
        Update($"Czas dojazdu: {travelTimeMs / 1000.0:F1}s");
        await Task.Delay(travelTimeMs);

        // Fałszywy alarm po dojeździe
        if (rand.NextDouble() < 0.05)
        {
            iterator = vehicleCollection.GetIterator();
            while (iterator.HasNext())
            {
                var v = iterator.Next();
                v.Return();
                Update($"Pojazd {v.Name} wraca do jednostki", v.State);
            }

            await Task.Delay(rand.Next(1000, 4000));

            iterator = vehicleCollection.GetIterator();
            while (iterator.HasNext())
            {
                var v = iterator.Next();
                v.Free();
                Update($"{v.Name} ponownie DOSTĘPNY w bazie", v.State);
            }

            _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;
            await CheckPendingEventsAsync();
            
            // po zakończeniu obsługi fałszywego alarmu lub powrotu pojazdów
            foreach (var obs in _observers.ToArray())
            {
                Detach(obs); // odpinamy np. loggera lub UnitObserver po zakończeniu eventu
            }

            return;
        }

        // Akcja na miejscu
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Arrive();
            Update($"Pojazd {v.Name} na miejscu zdarzenia", v.State);
        }

        await Task.Delay(rand.Next(5000, 25000));

        // Powrót
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Return();
            Update($"Pojazd {v.Name} wraca do jednostki", v.State);
        }

        await Task.Delay(rand.Next(1000, 4000));

        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Free();
            Update($"Pojazd {v.Name} dostępny ponownie", v.State);
        }

        _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;
        await CheckPendingEventsAsync();
    }

    private Task CheckPendingEventsAsync()
    {
        if (_pendingEvents.TryDequeue(out var nextEvent))
        {
            _ = Task.Run(() => HandleEventAsync(nextEvent));
        }

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