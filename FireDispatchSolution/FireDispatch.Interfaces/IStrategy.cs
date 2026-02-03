using FireDispatch.Models;

namespace FireDispatch.Interfaces;

// Dispatch strategy interface.
// Implementations return a list of vehicles to be dispatched for a given event.
public interface IStrategy
{
    // Designates vehicles to be dispatched for the event.
    // Parameters: unit list, event, number of required vehicles.
    // Returns a list of vehicles in the order they were assigned.
    IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount);
}