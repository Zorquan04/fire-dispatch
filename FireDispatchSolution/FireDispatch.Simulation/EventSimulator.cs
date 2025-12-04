using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;
using FireDispatch.Strategy;
using System.Collections.Concurrent;
using FireDispatch.Observer;

namespace FireDispatch.Simulation;

// Klasa odpowiedzialna za całą logikę symulacji zdarzeń.
// Obsługuje nowe zgłoszenia, przydziela pojazdy, symuluje dojazd, akcję i powrót do remizy.
// Implementuje IObserver aby przekazywać logi do innych obserwatorów.
public class EventSimulator(Dispatcher dispatcher, CommandCenter commandCenter) : IObserver
{
    // Lista obserwatorów symulatora (np. logger, unit observer)
    private readonly List<IObserver> _observers = new();

    // Kolejka zdarzeń czekających na wolne pojazdy
    private readonly ConcurrentQueue<Event> _pendingEvents = new();

    // Statystyki symulacji
    private int _totalEvents;
    private int _totalVehiclesDispatched;
    private int _totalTimeMs;

    // Rejestracja obserwatora (Logger, UnitObserver)
    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);

    // Przekazanie komunikatu do wszystkich obserwatorów
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        foreach (var obs in _observers.ToArray())
            obs.Update(message, vehicle, state);
    }

    // Główna metoda obsługi zgłoszenia - uruchamiana asynchronicznie
    public async Task HandleEventAsync(Event evt)
    {
        var rand = new Random();

        // Opóźnienie startu żeby symulacja wyglądała naturalniej
        await Task.Delay(rand.Next(200, 500));

        // Informujemy centrum dowodzenia o nowym wydarzeniu
        commandCenter.NewEvent(evt);

        var eventStartTime = DateTime.Now;
        Interlocked.Increment(ref _totalEvents);

        Update($"--- NOWE ZDARZENIE: {evt.Label} ---");
        Update($"Lokalizacja: {evt.Location.Latitude:F5}, {evt.Location.Longitude:F5}");

        // Liczba pojazdów wymaganych zależnie od typu zgłoszenia
        int requiredCount = evt.Type switch
        {
            EventType.Pz => 3, // Pożar → większe siły
            EventType.Mz => 2, // Miejscowe zagrożenie → standard
            EventType.Af => 0, // Alarm fałszywy → brak wyjazdu
            _ => 2
        };

        // Jeśli fałszywy alarm od początku → kończymy
        if (requiredCount == 0)
        {
            Update("Zgłoszenie oznaczone jako FAŁSZYWE – brak wysyłania pojazdów.");
            return;
        }

        // Próbujemy wysłać pojazdy
        var vehicles = dispatcher.Dispatch(evt, requiredCount).ToList();

        // Jeśli brak wolnych – odkładamy na później
        if (!vehicles.Any())
        {
            Update("Brak wolnych pojazdów – dodaję zdarzenie do kolejki oczekujących");
            _pendingEvents.Enqueue(evt);
            return;
        }

        Interlocked.Add(ref _totalVehiclesDispatched, vehicles.Count);

        // Tworzymy kolekcję pojazdów z iteratorami
        var vehicleCollection = new VehicleCollection(vehicles);

        // 1) Przypisanie pojazdów
        var iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Assign();
            Update($"Pojazd {v.Name} przypisany do zdarzenia {evt.Label}", v, v.State);
        }

        // 2) Wyjazd i dojazd na miejsce
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

        // Losowa szansa fałszywego alarmu (tutaj 5%)
        bool falseAlarm = rand.Next(100) < 5;

        // 3) Działania na miejscu zdarzenia
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Arrive();
            Update($"Pojazd {v.Name} na miejscu zdarzenia {evt.Label}", v, v.State);

            // Jeśli fałszywy – zawracamy bez akcji
            if (falseAlarm)
            {
                Update($"FAŁSZYWY ALARM przy {evt.Label}! Jednostki zawracają.", v, v.State);
                v.Return();
                Update($"Pojazd {v.Name} wraca do jednostki (fałszywy alarm)", v, v.State);
                continue;
            }

            // Normalna akcja
            int actionTimeMs = rand.Next(5000, 15000);
            Update($"Czas aktywności pojazdu {v.Name}: {actionTimeMs / 1000.0:F1}s");
            await Task.Delay(actionTimeMs);
        }

        // 4) Powrót do bazy
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

        // 5) Zwolnienie pojazdów po powrocie
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Free();
            Update($"Pojazd {v.Name} dostępny ponownie", v, v.State);
        }

        // Zapis czasu całego zdarzenia do statystyk
        _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;

        // Sprawdź czy w kolejce czekają inne zgłoszenia
        await CheckPendingEventsAsync();
    }

    // Obsługa kolejnych zdarzeń oczekujących jeśli zwolniły się pojazdy
    private Task CheckPendingEventsAsync()
    {
        if (_pendingEvents.TryDequeue(out var nextEvent))
            _ = Task.Run(() => HandleEventAsync(nextEvent));

        return Task.CompletedTask;
    }

    // Prosty wydruk końcowych statystyk
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