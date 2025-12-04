using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Strategy;

// Strategia wyboru pojazdów równoważąca obciążenie jednostek.
// Wybiera pojazdy z jednostki, która ma najwięcej wolnych aut.
public class BalancedStrategy : IStrategy
{
    public IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount)
    {
        // Wybieramy jednostkę z największą liczbą wolnych pojazdów
        var bestUnit = units
            .Where(u => u.FreeVehicleCount() > 0)           // tylko jednostki z wolnymi pojazdami
            .OrderByDescending(u => u.FreeVehicleCount())   // sortowanie malejąco po wolnych pojazdach
            .FirstOrDefault();                                  // wybieramy pierwszą (najlepszą) jednostkę

        if (bestUnit == null)
            return []; // brak dostępnych pojazdów w żadnej jednostce

        // Z tej jednostki wybieramy określoną liczbę wolnych pojazdów
        return bestUnit.Vehicles
            .Where(v => v.State == VehicleState.Free)
            .Take(requiredCount)
            .ToList();
    }
}