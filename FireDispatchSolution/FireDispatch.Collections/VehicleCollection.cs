using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Collections;

public class VehicleCollection(List<Vehicle> vehicles) : IAggregate<Vehicle>
{
    public IIterator<Vehicle> GetIterator() => new VehicleIterator(vehicles);

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