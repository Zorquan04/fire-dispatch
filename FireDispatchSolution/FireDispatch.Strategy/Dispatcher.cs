using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;

namespace FireDispatch.Strategy;

// Klasa Dispatcher odpowiada za przydzielanie pojazdów do zdarzeń
// Zawiera logikę wyboru pojazdów przy użyciu strategii (NearestFirst lub Balanced)
public class Dispatcher(IStrategy strategy, UnitCollection units)
{
    private IStrategy _strategy = strategy;

    // Zmiana strategii w trakcie działania (np. z NearestFirst na Balanced)
    private void SetStrategy(IStrategy strategy) => _strategy = strategy;

    // Główna metoda przydzielania pojazdów do zdarzenia
    public IEnumerable<Vehicle> Dispatch(Event evt, int requiredCount)
    {
        // Próba wybrania pojazdów według aktualnej strategii
        var vehicles = _strategy.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();

        // Jeśli brak pojazdów i obecna strategia to NearestFirst → zmień na Balanced
        if (!vehicles.Any() && _strategy is NearestFirstStrategy)
        {
            var balanced = new BalancedStrategy();
            vehicles = balanced.SelectVehicles(units.AsEnumerable(), evt, requiredCount).ToList();
            
            // Jeśli Balanced znalazł pojazdy, ustawiamy nową strategię na przyszłość
            if (vehicles.Any())
                SetStrategy(balanced);
        }

        return vehicles; // zwracamy przydzielone pojazdy
    }
}