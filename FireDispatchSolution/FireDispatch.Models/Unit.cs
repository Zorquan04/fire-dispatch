namespace FireDispatch.Models;

// Jednostka ratownicza (stacja) zawierająca listę pojazdów i położenie.
public class Unit(string name, Location location)
{
    private Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public Location Location { get; } = location;

    private readonly List<Vehicle> _vehicles = [];

    public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);

    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    public int FreeVehicleCount() => _vehicles.Count(v => v.State == VehicleState.Free);

    public override string ToString() => $"Unit {{Id={Id}, Name={Name}, FreeVehicles={FreeVehicleCount()}}}";
}