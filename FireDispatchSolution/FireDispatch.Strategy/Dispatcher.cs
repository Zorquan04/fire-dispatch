using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;

namespace FireDispatch.Strategy;

public class Dispatcher(IStrategy strategy, UnitCollection units)
{
    private IStrategy _strategy = strategy;

    private void SetStrategy(IStrategy strategy) => _strategy = strategy;

    public IEnumerable<Vehicle> Dispatch(Event evt, int requiredCount)
    {
        var vehicles = _strategy.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();

        // jeśli brak pojazdów i aktualna strategia to NearestFirst → zmień na Balanced
        if (!vehicles.Any() && _strategy is NearestFirstStrategy)
        {
            var balanced = new BalancedStrategy();
            vehicles = balanced.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();
            if (vehicles.Any())
                SetStrategy(balanced);
        }

        return vehicles;
    }
}