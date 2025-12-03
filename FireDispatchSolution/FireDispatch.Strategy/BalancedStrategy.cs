using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Strategy;

// Strategia równoważąca — wybiera pojazdy z jednostki mającej najwięcej wolnych aut.
public class BalancedStrategy : IStrategy
{
    public IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount)
    {
        var bestUnit = units.Where(u => u.FreeVehicleCount() > 0).OrderByDescending(u => u.FreeVehicleCount()).FirstOrDefault();

        if (bestUnit == null)
            return [];

        return bestUnit.Vehicles.Where(v => v.State == VehicleState.Free).Take(requiredCount).ToList();
    }
}