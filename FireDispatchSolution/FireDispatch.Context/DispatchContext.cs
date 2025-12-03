using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Context
{
    /// Dispatcher — kontekst strategii dysponowania
    public class DispatchContext(IStrategy strategy, List<Unit> units)
    {
        private IStrategy _strategy = strategy;

        public void SetStrategy(IStrategy strategy) => _strategy = strategy;

        // Zwraca listę pojazdów do wysłania na zdarzenie, delegując do strategii
        public List<Vehicle> Dispatch(Event evt, int requiredCount) => _strategy.SelectVehicles(units, evt, requiredCount).ToList();
    }
}