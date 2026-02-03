using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

public class UnitCollection : IAggregate<Unit>
{
    // Maintains a list of fire departments
    private readonly List<Unit> _units = new();

    // Adding a unit to the collection
    public void Add(Unit unit) => _units.Add(unit);

    // Returns an iterator that allows iterating over entities without revealing the implementation of the list
    public IIterator<Unit> GetIterator() => new UnitIterator(_units);

    // Inner iterator class that operates on a list of entities
    private class UnitIterator(List<Unit> units) : IIterator<Unit>
    {
        private int _index;

        // Checks if there is another item in the collection
        public bool HasNext() => _index < units.Count;

        // Returns the next unit by moving the iterator pointer
        public Unit Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more units.");
            return units[_index++];
        }
    }
    
    // A convenient method to iterate through foreach without manually referencing the iterator.
    public IEnumerable<Unit> AsEnumerable()
    {
        var iterator = GetIterator();
        while (iterator.HasNext())
            yield return iterator.Next();
    }
}