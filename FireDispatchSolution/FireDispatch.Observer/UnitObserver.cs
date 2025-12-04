using FireDispatch.Models;
using FireDispatch.Interfaces;

namespace FireDispatch.Observer;

// Obserwator nasłuchujący zmian dotyczących konkretnej jednostki straży
// Filtruje tylko komunikaty powiązane z jej pojazdami
public class UnitObserver(Unit unit, IObserver logger) : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        // Logujemy tylko, jeśli zdarzenie dotyczy pojazdu z tej jednostki
        if (vehicle != null && vehicle.Unit == unit)
        {
            // Przekazujemy komunikat dalej do loggera (najczęściej ConsoleLogger)
            logger.Update($"[{unit.Name}] {message}", null, state);
        }
    }
}