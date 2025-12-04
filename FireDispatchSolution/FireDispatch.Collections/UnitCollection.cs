using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

public class UnitCollection : IAggregate<Unit>
{
    // Przechowuje listę jednostek straży pożarnej
    private readonly List<Unit> _units = new();

    // Dodawanie jednostki do kolekcji
    public void Add(Unit unit) => _units.Add(unit);

    // Zwraca iterator umożliwiający iterację po jednostkach bez ujawniania implementacji listy
    public IIterator<Unit> GetIterator() => new UnitIterator(_units);

    // Wewnętrzna klasa iteratora operująca na liście jednostek
    private class UnitIterator(List<Unit> units) : IIterator<Unit>
    {
        private int _index;

        // Sprawdza, czy istnieje kolejny element w kolekcji
        public bool HasNext() => _index < units.Count;

        // Zwraca kolejną jednostkę, przesuwając wskaźnik iteratora
        public Unit Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more units.");
            return units[_index++];
        }
    }
    
    // Wygodna metoda umożliwiająca iterowanie foreach bez ręcznego odwoływania do iteratora
    public IEnumerable<Unit> AsEnumerable()
    {
        var iterator = GetIterator();
        while (iterator.HasNext())
            yield return iterator.Next();
    }
}