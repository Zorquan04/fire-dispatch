using System.Collections;
using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

// Vehicle Collection - implements the Iterator pattern and allows foreach (IEnumerable)
// Takes a Vehicle collection and internally stores them in a list
public class VehicleCollection(IEnumerable<Vehicle> vehicles) : IAggregate<Vehicle>, IEnumerable
{
    // Internal vehicle list (copied from input collection)
    private readonly List<Vehicle> _vehicles = vehicles.ToList();

    // Returns an iterator that conforms to our IIterator<T> interface
    public IIterator<Vehicle> GetIterator() => new VehicleIterator(_vehicles);

    // Custom enumerator under foreach – iterates through the list of vehicles
    private IEnumerator<Vehicle> GetEnumerator()
    {
        foreach (var v in _vehicles)
            yield return v; // yield allows you to return an item without creating a temporary array
    }

    // IEnumerable implementation – required for foreach
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    // Internal Index-Based Iterator – Classic Iterator Pattern
    private class VehicleIterator(List<Vehicle> vehicles) : IIterator<Vehicle>
    {
        private int _index; // current position in the list

        // Is there anything else to download?
        public bool HasNext() => _index < vehicles.Count;

        // Returns the next item and moves the cursor
        public Vehicle Next()
        {
            if (!HasNext()) throw new InvalidOperationException("No more vehicles.");
            return vehicles[_index++];
        }
    }
}