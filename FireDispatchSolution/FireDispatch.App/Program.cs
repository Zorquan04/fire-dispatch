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

        var unit1 = new Unit("JRG-1", new Location(50.05, 19.94));
        var unit2 = new Unit("JRG-2", new Location(50.04, 19.92));

        for (int i = 1; i <= 5; i++)
        {
            unit1.AddVehicle(new Vehicle($"JRG1-V{i}", unit1));
            unit2.AddVehicle(new Vehicle($"JRG2-V{i}", unit2));
        }

        var unitCollection = new UnitCollection();
        unitCollection.Add(unit1);
        unitCollection.Add(unit2);

        var dispatcher = new Dispatcher(new NearestFirstStrategy(), unitCollection);
        var simulator = new EventSimulator(dispatcher);

        var consoleLogger = new ConsoleLogger();
        simulator.Attach(consoleLogger);

        // dodajemy obserwatorów jednostek (tylko logowanie)
        simulator.Attach(new UnitObserver(unit1, consoleLogger));
        simulator.Attach(new UnitObserver(unit2, consoleLogger));

        var rng = new Random();
        int eventCount = 10;

        for (int i = 0; i < eventCount; i++)
        {
            EventType type = rng.NextDouble() < 0.7 ? EventType.Mz : EventType.Pz;
            double lat = 50.04 + rng.NextDouble() * 0.02;
            double lon = 19.92 + rng.NextDouble() * 0.02;

            var evt = new Event(type, new Location(lat, lon));
            _ = simulator.HandleEventAsync(evt);

            // Realistyczny odstęp między zgłoszeniami
            await Task.Delay(rng.Next(1000, 3000));
        }

        // poczekaj na zakończenie równoległych zadań
        await Task.Delay(30000); // dajemy czas na wszystkie akcje
        simulator.PrintStatistics();
        simulator.Detach(consoleLogger);
        Console.WriteLine("Symulacja zakończona");
    }
}