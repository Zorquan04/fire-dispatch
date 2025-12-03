using FireDispatch.Context;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// Observer jednostki PSP – reaguje na powiadomienia SKKM
// Teraz przy powiadomieniu może wybrać pojazdy i wysłać je do zdarzenia
public class UnitObserver(Unit unit, DispatchContext dispatcher, IObserver logger) : IObserver
{
    public void Update(string message, VehicleState? state = null)
    {
        logger.Update($"[{unit.Name}] {message}", state);

        // Przykładowe zdarzenie powiązane z wiadomością
        var evt = new Event(EventType.Pz, unit.Location); // dla demo – zdarzenie przy lokalizacji jednostki
        var vehicles = dispatcher.Dispatch(evt, 2); // np. 2 pojazdy

        foreach (var v in vehicles)
        {
            v.Assign();
            logger.Update($"Wysyłam pojazd {v.Name} do zdarzenia {evt.Type}", v.State);

            v.StartTravel();
            logger.Update($"Pojazd {v.Name} w drodze", v.State);

            v.Arrive();
            logger.Update($"Pojazd {v.Name} na miejscu zdarzenia", v.State);

            v.Return();
            logger.Update($"Pojazd {v.Name} wraca do jednostki", v.State);

            v.Free();
            logger.Update($"Pojazd {v.Name} dostępny", v.State);
        }
    }
}