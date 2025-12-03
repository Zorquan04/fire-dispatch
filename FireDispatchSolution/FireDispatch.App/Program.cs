using FireDispatch.Models;

namespace FireDispatch.App;

/// Główny punkt wejścia aplikacji. Ten plik powstał w commicie inicjalnym.
/// W kolejnych commitach podłaczymy tu symulator i moduły.
internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("FireDispatch — demo tworzenia jednostek");

        // Tworzymy jednostki z pojazdami
        var units = new List<Unit>
        {
            new Unit("JRG-1", new Location(50.05,19.94))
            {
                Vehicles =
                {
                    new Vehicle("JRG1-V1", Guid.NewGuid()),
                    new Vehicle("JRG1-V2", Guid.NewGuid()),
                    new Vehicle("JRG1-V3", Guid.NewGuid()),
                    new Vehicle("JRG1-V4", Guid.NewGuid()),
                    new Vehicle("JRG1-V5", Guid.NewGuid())
                }
            },
            new Unit("JRG-2", new Location(50.03,19.91))
            {
                Vehicles =
                {
                    new Vehicle("JRG2-V1", Guid.NewGuid()),
                    new Vehicle("JRG2-V2", Guid.NewGuid()),
                    new Vehicle("JRG2-V3", Guid.NewGuid())
                }
            },
            new Unit("JRG-3", new Location(50.09,19.99))
            {
                Vehicles =
                {
                    new Vehicle("JRG3-V1", Guid.NewGuid()),
                    new Vehicle("JRG3-V2", Guid.NewGuid()),
                    new Vehicle("JRG3-V3", Guid.NewGuid()),
                    new Vehicle("JRG3-V4", Guid.NewGuid())
                }
            }
        };

        var evt = new Event(EventType.Pz, new Location(50.045,19.93));

        // Test 1 — najbliższa jednostka
        var dispatcher = new Dispatcher(new NearestFirstStrategy());
        var selected = dispatcher.Dispatch(units, evt, 3);

        Console.WriteLine("Nearest strategy:");
        foreach (var v in selected) Console.WriteLine($"Vehicle {v.Name} ({v.Id})");

        // Test 2 — zmiana strategii
        dispatcher.SetStrategy(new BalancedStrategy());
        var selectedBalanced = dispatcher.Dispatch(units, evt, 2);

        Console.WriteLine("\nBalanced strategy:");
        foreach (var v in selectedBalanced) Console.WriteLine($"Vehicle {v.Name} ({v.Id})");
    }
}