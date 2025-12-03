using FireDispatch.Interfaces;
using FireDispatch.Models;

/// Strategia wybierająca pojazdy z jednostki najbliższej zdarzeniu.
public class NearestFirstStrategy : IStrategy
{
    public IEnumerable<Vehicle> SelectVehicles(IEnumerable<Unit> units, Event evt, int requiredCount)
    {
        // Najpierw szukamy najbliższej jednostki z wolnymi pojazdami
        var nearestUnit = units.Where(u => u.FreeVehicleCount() > 0).OrderBy(u => u.Location.DistanceTo(evt.Location)).FirstOrDefault();

        if (nearestUnit == null)
            return Enumerable.Empty<Vehicle>(); // brak wolnych

        // Pobieramy tyle pojazdów ile potrzeba (albo tyle ile mamy)
        return nearestUnit.Vehicles.Where(v => v.State == VehicleState.Free).Take(requiredCount).ToList();
    }
}