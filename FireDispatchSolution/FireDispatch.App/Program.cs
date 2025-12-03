using FireDispatch.Models;
using FireDispatch.Observer;

namespace FireDispatch.App;

/// Główny punkt wejścia aplikacji
internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("FireDispatch — demo tworzenia jednostek");

        // Tworzymy jednostki
        var jrg1 = new Unit("JRG-1", new Location(50.0647, 19.9450));
        var jrg2 = new Unit("JRG-2", new Location(50.0700, 19.9333));

        // Dodajemy pojazdy
        for (int i = 1; i <= 5; i++)
        {
            jrg1.Vehicles.Add(new Vehicle($"JRG1-V{i}", jrg1.Id));
            jrg2.Vehicles.Add(new Vehicle($"JRG2-V{i}", jrg2.Id));
        }

        // Tworzymy obserwatorów jednostek
        var observer1 = new UnitObserver(jrg1);
        var observer2 = new UnitObserver(jrg2);

        // Tworzymy SKKM (Subject)
        var skkm = new CommandCenter();
        skkm.Attach(observer1);
        skkm.Attach(observer2);

        // Wywołanie zdarzenia
        skkm.NewEvent("Pożar w okolicy Placu Matejki PZ");
    }
}