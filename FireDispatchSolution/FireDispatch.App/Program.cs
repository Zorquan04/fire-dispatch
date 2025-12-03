using FireDispatch.Context;
using FireDispatch.Models;
using FireDispatch.Observer;
using FireDispatch.Simulation;
using FireDispatch.Strategy;

namespace FireDispatch.App;

// Główny punkt wejścia aplikacji
internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("FireDispatch — symulacja pętli zdarzeń");

        // Jednostki
        var unit1 = new Unit("JRG-1", new Location(50.05, 19.94));
        var unit2 = new Unit("JRG-2", new Location(50.04, 19.92));

        for (int i = 1; i <= 5; i++)
        {
            unit1.Vehicles.Add(new Vehicle($"JRG1-V{i}", unit1.Id));
            unit2.Vehicles.Add(new Vehicle($"JRG2-V{i}", unit2.Id));
        }

        var units = new List<Unit> { unit1, unit2 };
        var dispatcher = new DispatchContext(new NearestFirstStrategy(), units);

        var simulator = new EventSimulator(dispatcher);
        simulator.Attach(new ConsoleLogger());

        // Generowanie przykładowych zdarzeń
        var events = new List<Event>
        {
            new (EventType.Pz, new Location(50.045, 19.93)),
            new (EventType.Mz, new Location(50.048, 19.935)),
            new (EventType.Af, new Location(50.046, 19.938))
        };

        // Pętla zdarzeń
        foreach (var evt in events)
        {
            await simulator.HandleEventAsync(evt);
        }

        Console.WriteLine("Symulacja zakończona");
    }
}