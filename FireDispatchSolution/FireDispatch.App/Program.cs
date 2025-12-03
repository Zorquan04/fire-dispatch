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
            unit1.AddVehicle(new Vehicle($"JRG1-V{i}"));
            unit2.AddVehicle(new Vehicle($"JRG2-V{i}"));
        }

        var unitCollection = new UnitCollection();
        unitCollection.Add(unit1);
        unitCollection.Add(unit2);

        var dispatcher = new Dispatcher(new NearestFirstStrategy(), unitCollection);
        var simulator = new EventSimulator(dispatcher);
        var consoleLogger = new ConsoleLogger();
        simulator.Attach(consoleLogger);

        var center = new CommandCenter();
        center.Attach(new UnitObserver(unit1, simulator, simulator));
        center.Attach(new UnitObserver(unit2, simulator, simulator));

        var rng = new Random();
        int eventCount = 5;

        for (int i = 0; i < eventCount; i++)
        {
            EventType type = rng.NextDouble() < 0.7 ? EventType.Mz : EventType.Pz;
            double lat = 50.04 + rng.NextDouble() * 0.02;
            double lon = 19.92 + rng.NextDouble() * 0.02;

            var evt = new Event(type, new Location(lat, lon));
            center.NewEvent(evt);

            int wait = rng.Next(0, 3000);
            await Task.Delay(wait);
        }

        // poczekaj na zakończenie równoległych zadań
        await Task.Delay(5000); 
        simulator.PrintStatistics();
        simulator.Detach(consoleLogger);
        Console.WriteLine("Symulacja zakończona");
    }
}