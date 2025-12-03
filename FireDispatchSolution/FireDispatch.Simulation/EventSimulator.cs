using FireDispatch.Context;
using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;

namespace FireDispatch.Simulation;

public class EventSimulator(DispatchContext dispatcher)
{
    private readonly List<IObserver> _observers = [];

    // Statystyki
    private int _totalEvents;
    private int _totalVehiclesDispatched;
    private int _totalTimeMs;

    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);
    
    private void Notify(string message, VehicleState? state = null)
    {
        foreach (var obs in _observers)
            obs.Update(message, state);
    }

    public async Task HandleEventAsync(Event evt)
    {
        var rand = new Random();
        var eventStartTime = DateTime.Now;

        _totalEvents++;

        Notify($"--- NOWE ZDARZENIE: {evt.Type} ---");
        Notify($"Lokalizacja: {evt.Location.Latitude}, {evt.Location.Longitude}");

        int requiredCount = evt.Type switch
        {
            EventType.Pz => 3, // Pożar – więcej sił
            EventType.Mz => 2, // Mniejsze zdarzenie
            EventType.Af => 0, // Fałszywy alarm z góry
            _ => 2
        };

        // alarm fałszywy natychmiast z polecenia EventType.Af
        if (requiredCount == 0)
        {
            Notify("Zgłoszenie oznaczone jako FAŁSZYWE – brak wysyłania pojazdów.");
            return;
        }

        // pobieramy pojazdy z dyspozytora
        var vehicles = dispatcher.Dispatch(evt, requiredCount);

        if (!vehicles.Any())
        {
            Notify("Brak wolnych pojazdów – zgłoszenie czeka lub przekazać do innej jednostki (Etap 2).");
            return;
        }

        _totalVehiclesDispatched += vehicles.Count;
        var vehicleCollection = new VehicleCollection(vehicles);

        // 1) przypisanie pojazdów
        foreach (var v in vehicleCollection)
        {
            v.Assign();
            Notify($"Pojazd {v.Name} przypisany do zdarzenia {evt.Type}", v.State);
        }

        // 2) wyjazd w drogę
        foreach (var v in vehicleCollection)
        {
            v.StartTravel();
            Notify($"Pojazd {v.Name} wyjechał – jedzie na miejsce", v.State);
        }

        int travelTimeMs = rand.Next(1000, 4000); // 1-4 sek
        Notify($"Czas dojazdu: {travelTimeMs / 1000.0:F1}s");
        await Task.Delay(travelTimeMs);

        // 3) Losowa szansa FAŁSZYWEGO alarmu dopiero po dojeździe (5%)
        bool falseAlarm = rand.NextDouble() < 0.05;
        if (falseAlarm)
        {
            Notify("Zgłoszenie okazało się FAŁSZYWE po przybyciu jednostki!");
            Notify("Oddział zawraca do bazy bez podejmowania działań.");

            foreach (var v in vehicleCollection)
            {
                v.Return();
                Notify($"Pojazd {v.Name} wraca do jednostki", v.State);
            }

            int backTime = rand.Next(1000, 4000);
            Notify($"Czas powrotu: {backTime/1000.0:F1}s");
            await Task.Delay(backTime);

            foreach (var v in vehicleCollection)
            {
                v.Free();
                Notify($"{v.Name} ponownie DOSTĘPNY w bazie", v.State);
            }

            _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;

            Notify($"FAŁSZYWY ALARM — czas od zgłoszenia do powrotu: {(DateTime.Now-eventStartTime).TotalSeconds:F1}s");
            Notify("---------------------------------------------------------");
            return;
        }

        // 4) dotarcie na miejsce
        foreach (var v in vehicleCollection)
        {
            v.Arrive();
            Notify($"Pojazd {v.Name} na miejscu zdarzenia", v.State);
        }

        int actionTimeMs = rand.Next(5000, 25000); // akcja 5-25s
        Notify($"Akcja gaśnicza potrwa ok. {actionTimeMs / 1000.0:F1}s");
        await Task.Delay(actionTimeMs);

        // 5) powrót
        foreach (var v in vehicleCollection)
        {
            v.Return();
            Notify($"Pojazd {v.Name} wraca do jednostki", v.State);
        }

        int returnTimeMs = rand.Next(1000, 4000);
        Notify($"Czas powrotu: {returnTimeMs / 1000.0:F1}s");
        await Task.Delay(returnTimeMs);

        foreach (var v in vehicleCollection)
        {
            v.Free();
            Notify($"Pojazd {v.Name} dostępny ponownie", v.State);
        }

        _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;
        Notify($"Zdarzenie zakończone. Czas całkowity: {(DateTime.Now-eventStartTime).TotalSeconds:F1}s");
        Notify("---------------------------------------------------------");
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