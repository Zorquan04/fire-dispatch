using FireDispatch.Collections;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Context
{
    // Dispatcher — kontekst strategii dysponowania
    public class DispatchContext(IStrategy strategy, UnitCollection units)
    {
        // Zwraca listę pojazdów do wysłania na zdarzenie, delegując do strategii
        public List<Vehicle> Dispatch(Event evt, int requiredCount)
        {
            var iterator = units.GetIterator(); // units typu UnitCollection
            var unitsList = new List<Unit>();

            while (iterator.HasNext())
                unitsList.Add(iterator.Next());

            return strategy.SelectVehicles(unitsList, evt, requiredCount).ToList();
        }
    }
}