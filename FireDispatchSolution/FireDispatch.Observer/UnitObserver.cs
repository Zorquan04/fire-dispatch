using FireDispatch.Models;
using FireDispatch.Interfaces;

namespace FireDispatch.Observer;

public class UnitObserver(Unit unit, IObserver logger) : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        // logujemy tylko jeśli zdarzenie dotyczy pojazdu tej jednostki
        if (vehicle != null && vehicle.Unit == unit)
        {
            logger.Update($"[{unit.Name}] {message}", null, state);
        }
    }
}