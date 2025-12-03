using FireDispatch.Models;
using FireDispatch.Context;

namespace FireDispatch.Simulation;

public class EventSimulator(DispatchContext dispatcher)
{
    private readonly DispatchContext _dispatcher = dispatcher;
    private readonly Random _rand = new();

    public async Task HandleEventAsync(Event evt)
    {
        // ile wozów potrzebujemy
        int required = evt.Type == EventType.Pz ? 3 : 2;

        var vehicles = _dispatcher.Dispatch(evt, required);
        if (vehicles.Count == 0)
        {
            Console.WriteLine($"Brak wolnych pojazdów dla zdarzenia {evt.Type}");
            return;
        }

        Console.WriteLine($"Dysponowanie {vehicles.Count} pojazdów do zdarzenia {evt.Type}");

        foreach (var v in vehicles)
        {
            v.Assign();
        }

        // losowy czas dojazdu 1-3 sekundy
        await Task.Delay(_rand.Next(1000, 3000));
        vehicles.ForEach(v => v.StartTravel());
        Console.WriteLine($"Pojazdy w drodze: {string.Join(", ", vehicles)}");

        // losowy czas akcji na miejscu 2-4 sekundy
        await Task.Delay(_rand.Next(2000, 4000));
        vehicles.ForEach(v => v.Arrive());
        Console.WriteLine($"Pojazdy na miejscu: {string.Join(", ", vehicles)}");

        // czas powrotu 1-2 sekundy
        await Task.Delay(_rand.Next(1000, 2000));
        vehicles.ForEach(v => v.Return());
        Console.WriteLine($"Pojazdy wracają: {string.Join(", ", vehicles)}");

        // wracamy do stanu Free
        vehicles.ForEach(v => v.Free());
        Console.WriteLine($"Pojazdy dostępne: {string.Join(", ", vehicles)}");
    }
}
