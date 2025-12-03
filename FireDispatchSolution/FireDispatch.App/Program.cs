using FireDispatch.Models;

namespace FireDispatch.App;

/// Główny punkt wejścia aplikacji. Ten plik powstał w commicie inicjalnym.
/// W kolejnych commitach podłaczymy tu symulator i moduły.
internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("FireDispatch — demo tworzenia jednostek");

        // Tworzymy przykładowe jednostki (lokacje przykładowe, nie rzeczywiste)
        var jrg1 = new Unit("JRG-1", new Location(50.0647, 19.9450));
        var jrg2 = new Unit("JRG-2", new Location(50.0700, 19.9333));
        
        // Dodajemy po 5 pojazdów do każdej jednostki
        for (int i = 1; i <= 5; i++)
        {
            jrg1.Vehicles.Add(new Vehicle($"JRG1-V{i}", jrg1.Id));
            jrg2.Vehicles.Add(new Vehicle($"JRG2-V{i}", jrg2.Id));
        }
        
        Console.WriteLine(jrg1);
        Console.WriteLine(jrg2);

        // Prosty event
        var evt = new Event(EventType.Pz, new Location(50.0650, 19.9400));
        Console.WriteLine($"Wygenerowano zdarzenie: {evt}");
    }
}