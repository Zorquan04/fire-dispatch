using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Strategy;
// Dispatcher (kontekst) — używa strategii do wyboru pojazdów.
public class Dispatcher(IStrategy strategy)
{
    private IStrategy _strategy = strategy;

    public void SetStrategy(IStrategy strategy) => _strategy = strategy;

    // Zwraca listę pojazdów wybranych przez strategię
    public IEnumerable<Vehicle> Dispatch(IEnumerable<Unit> units, Event evt, int requiredCars)
    {
        return _strategy.SelectVehicles(units, evt, requiredCars);
    }
}