using FireDispatch.Models;

namespace FireDispatch.Interfaces;

// Interfejs strategii dysponowania.
// Implementacje zwracają listę pojazdów do zadysponowania dla danego zdarzenia.
public interface IStrategy
{
    // Wyznacza pojazdy do dysponowania dla zdarzenia.
    // Parametry: lista jednostek, zdarzenie, liczba wymaganych pojazdów.
    // Zwraca listę pojazdów w kolejności przypisania.
    IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount);
}