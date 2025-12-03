using System.Collections;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

public class VehicleCollection(IEnumerable<Vehicle> vehicles) : IAggregate<Vehicle>, IEnumerable
{
    private readonly List<Vehicle> _vehicles = vehicles.ToList();

    public IIterator<Vehicle> GetIterator() => new VehicleIterator(_vehicles);

    private IEnumerator<Vehicle> GetEnumerator()
    {
        foreach (var v in _vehicles)
            yield return v;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    private class VehicleIterator(List<Vehicle> vehicles) : IIterator<Vehicle>
    {
        private int _index;

        public bool HasNext() => _index < vehicles.Count;

        public Vehicle Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more vehicles.");
            return vehicles[_index++];
        }
    }
}