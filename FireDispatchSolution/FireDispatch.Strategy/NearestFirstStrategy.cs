using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Strategy;

// Strategy that selects vehicles from the unit closest to the event.
public class NearestFirstStrategy : IStrategy
{
    public IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount)
    {
        // First, we look for the nearest unit with available vehicles
        var nearestUnit = units.Where(u => u.FreeVehicleCount() > 0).OrderBy(u => u.Location.DistanceTo(evt.Location)).FirstOrDefault();

        if (nearestUnit == null)
            return []; // no vacancies

        //We take as many vehicles as needed (or as many as we have)
        return nearestUnit.Vehicles.Where(v => v.State == VehicleState.Free).Take(requiredCount).ToList();
    }
}