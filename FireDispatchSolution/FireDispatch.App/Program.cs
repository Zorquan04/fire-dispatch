using FireDispatch.Collections;
using FireDispatch.Models;
using FireDispatch.Observer;
using FireDispatch.Simulation;
using FireDispatch.Strategy;

namespace FireDispatch.App;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("FireDispatch — symulacja zdarzeń losowych\n");

        // Tworzymy jednostki straży pożarnej
        // Przyjmujemy lokalizacje bazowe jako współrzędne GPS
        var unit1 = new Unit("JRG-1", new Location(50.05, 19.94));
        var unit2 = new Unit("JRG-2", new Location(50.04, 19.92));

        // Dodajemy pojazdy do każdej jednostki (po 5 na każdą)
        // Nazwa generowana dynamicznie np. JRG1-V1, JRG1-V2...
        for (int i = 1; i <= 5; i++)
        {
            unit1.AddVehicle(new Vehicle($"JRG1-V{i}", unit1));
            unit2.AddVehicle(new Vehicle($"JRG2-V{i}", unit2));
        }

        // Kolekcja jednostek – iterowana później przez strategię wyboru pojazdów
        var unitCollection = new UnitCollection();
        unitCollection.Add(unit1);
        unitCollection.Add(unit2);

        // Dispatcher odpowiada za wybór jednostek/pojazdów do zdarzenia
        // Wstrzykujemy strategię – tutaj najbliższy pojazd jako priorytet
        var dispatcher = new Dispatcher(new NearestFirstStrategy(), unitCollection);

        // SKKM – centrum dowodzenia (Mediator/Observer Subject)
        var commandCenter = new CommandCenter();

        // Symulator zdarzeń – główny orkiestrator akcji
        var simulator = new EventSimulator(dispatcher, commandCenter);
        
        // Logger konsolowy – wyświetla wszystkie komunikaty w aplikacji
        var consoleLogger = new ConsoleLogger();
        simulator.Attach(consoleLogger);
        commandCenter.Attach(consoleLogger);

        // Podpinamy obserwatorów jednostek – ich zadaniem jest logowanie działań pojazdów
        simulator.Attach(new UnitObserver(unit1, consoleLogger));
        simulator.Attach(new UnitObserver(unit2, consoleLogger));
        commandCenter.Attach(new UnitObserver(unit1, consoleLogger));
        commandCenter.Attach(new UnitObserver(unit2, consoleLogger));

        // Generator losowy + liczba zdarzeń w symulacji
        var rng = new Random();
        int eventCount = 10;

        // Generujemy zdarzenia co pewien losowy czas (symulacja rzeczywistości)
        for (int i = 0; i < eventCount; i++)
        {
            // W 70% przypadków MZ, 30% PZ
            EventType type = rng.NextDouble() < 0.7 ? EventType.Mz : EventType.Pz;

            // Losowanie lokalizacji zdarzenia w obrębie miasta
            double lat = 50.04 + rng.NextDouble() * 0.02;
            double lon = 19.92 + rng.NextDouble() * 0.02;

            // Tworzymy zgłoszenie i wysyłamy do symulatora asynchronicznie
            var evt = new Event(type, new Location(lat, lon));
            _ = simulator.HandleEventAsync(evt);

            // Krótka przerwa między zgłoszeniami
            await Task.Delay(rng.Next(1000, 3000));
        }

        // Czekamy aż wszystkie akcje zostaną wykonane
        await Task.Delay(30000);

        // Wypisujemy statystyki z symulacji (czas, ilość zdarzeń itp.)
        simulator.PrintStatistics();
        simulator.Detach(consoleLogger);

        Console.WriteLine("Symulacja zakończona");
    }
}