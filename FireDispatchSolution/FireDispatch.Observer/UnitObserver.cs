using FireDispatch.Context;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

/// Observer jednostki PSP – reaguje na powiadomienia SKKM
/// Teraz przy powiadomieniu może wybrać pojazdy i wysłać je do zdarzenia
public class UnitObserver(Unit unit, DispatchContext dispatcher) : IObserver
{
    public void Update(string message)
    {
        Console.WriteLine($"[{unit.Name}] Otrzymano powiadomienie: {message}");

        // Przykładowe zdarzenie powiązane z wiadomością
        var evt = new Event(EventType.Pz, unit.Location); // dla demo – zdarzenie przy lokalizacji jednostki
        var vehicles = dispatcher.Dispatch(evt, 2); // np. 2 pojazdy

        foreach (var v in vehicles)
        {
            Console.WriteLine($"[{unit.Name}] Wysyłam pojazd {v.Name} do zdarzenia {evt.Type}");
            v.Assign(); // zmiana stanu pojazdu
        }
    }
}