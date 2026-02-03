using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Strategy;

// Vehicle selection strategy that balances unit load.
// Selects vehicles from the unit with the most available vehicles.
public class BalancedStrategy : IStrategy
{
    public IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount)
    {
        // Select the unit with the most free vehicles
        var bestUnit = units
            .Where(u => u.FreeVehicleCount() > 0)           // only units with free vehicles
            .OrderByDescending(u => u.FreeVehicleCount())   // sort descending by free vehicles
            .FirstOrDefault();                                  // select the first (best) unit

        if (bestUnit == null)
            return []; // no vehicles available in any unit

        // From this unit we select a certain number of available vehicles
        return bestUnit.Vehicles
            .Where(v => v.State == VehicleState.Free)
            .Take(requiredCount)
            .ToList();
    }
}