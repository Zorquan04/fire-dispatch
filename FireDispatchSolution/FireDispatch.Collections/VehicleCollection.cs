using System.Collections;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

// Kolekcja pojazdów – implementuje wzorzec Iterator oraz umożliwia foreach (IEnumerable)
// Przyjmuje kolekcję Vehicle i wewnętrznie trzyma je w liście
public class VehicleCollection(IEnumerable<Vehicle> vehicles) : IAggregate<Vehicle>, IEnumerable
{
    // Wewnętrzna lista pojazdów (kopiowana z wejściowej kolekcji)
    private readonly List<Vehicle> _vehicles = vehicles.ToList();

    // Zwraca iterator zgodny z naszym interfejsem IIterator<T>
    public IIterator<Vehicle> GetIterator() => new VehicleIterator(_vehicles);

    // Własny enumerator pod foreach – iteruje po liście pojazdów
    private IEnumerator<Vehicle> GetEnumerator()
    {
        foreach (var v in _vehicles)
            yield return v; // yield pozwala zwrócić element bez tworzenia tablicy tymczasowej
    }

    // Implementacja IEnumerable – wymagane do foreach
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    // Wewnętrzny iterator oparty o indeks – klasyczny Iterator Pattern
    private class VehicleIterator(List<Vehicle> vehicles) : IIterator<Vehicle>
    {
        private int _index; // aktualna pozycja w liście

        // Czy jest jeszcze coś do pobrania?
        public bool HasNext() => _index < vehicles.Count;

        // Zwraca kolejny element i przesuwa kursor
        public Vehicle Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more vehicles.");
            return vehicles[_index++];
        }
    }
}