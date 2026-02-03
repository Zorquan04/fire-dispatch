using FireDispatch.Models;
using FireDispatch.Interfaces;

namespace FireDispatch.Observer;

// Observer that listens for changes related to a specific fire department
// Filters only messages related to its vehicles
public class UnitObserver(Unit unit, IObserver logger) : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        // We only log if the event concerns a vehicle from this unit
        if (vehicle != null && vehicle.Unit == unit)
        {
            // We pass the message on to the logger (usually ConsoleLogger)
            logger.Update($"[{unit.Name}] {message}", null, state);
        }
    }
}