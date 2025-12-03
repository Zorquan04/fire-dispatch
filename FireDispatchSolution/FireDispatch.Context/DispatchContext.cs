using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Context
{
    /// Dispatcher — kontekst strategii dysponowania
    public class DispatchContext(IStrategy strategy, IEnumerable<Unit> units)
    {
        public IStrategy Strategy { get; private set; } = strategy; // aktualnie używana strategia

        // wszystkie jednostki PSP
        public void SetStrategy(IStrategy strategy) => Strategy = strategy;
        
        /// Zwraca listę pojazdów do wysłania na zdarzenie, delegując do strategii
        public IEnumerable<Vehicle> Dispatch(Event evt, int requiredCount)
        {
            return Strategy.SelectVehicles(units, evt, requiredCount);
        }
    }
}