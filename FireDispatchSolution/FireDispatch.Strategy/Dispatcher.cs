using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;

namespace FireDispatch.Strategy;

// The Dispatcher class is responsible for assigning vehicles to events
// Contains the logic for selecting vehicles using a strategy (NearestFirst or Balanced)
public class Dispatcher(IStrategy strategy, UnitCollection units)
{
    private IStrategy _strategy = strategy;

    // Changing strategy during operation (e.g. from NearestFirst to Balanced)
    private void SetStrategy(IStrategy strategy) => _strategy = strategy;

    // The main method of assigning vehicles to an incident
    public IEnumerable<Vehicle> Dispatch(Event evt, int requiredCount)
    {
        // Attempt to select vehicles according to the current strategy
        var vehicles = _strategy.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();

        // If there are no vehicles and the current strategy is NearestFirst → change to Balanced
        if (!vehicles.Any() && _strategy is NearestFirstStrategy)
        {
            var balanced = new BalancedStrategy();
            vehicles = balanced.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();
            
            // If Balanced has found the vehicles, we are setting a new strategy for the future
            if (vehicles.Any())
                SetStrategy(balanced);
        }

        return vehicles; // we return the assigned vehicles
    }
}