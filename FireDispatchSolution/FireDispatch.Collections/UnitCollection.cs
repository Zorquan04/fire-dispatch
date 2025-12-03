using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

public class UnitCollection : IAggregate<Unit>
{
    private readonly List<Unit> _units = new();

    public void Add(Unit unit) => _units.Add(unit);

    public IIterator<Unit> GetIterator() => new UnitIterator(_units);

    private class UnitIterator(List<Unit> units) : IIterator<Unit>
    {
        private int _index;

        public bool HasNext() => _index < units.Count;

        public Unit Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more units.");
            return units[_index++];
        }
    }
}