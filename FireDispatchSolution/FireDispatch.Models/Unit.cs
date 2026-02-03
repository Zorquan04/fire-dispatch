namespace FireDispatch.Models;

// Fire brigade unit (JRG/OSP) with vehicles and coordinates
public class Unit(string name, Location location)
{
    // Randomly generated unit ID
    private Guid Id { get; } = Guid.NewGuid();

    // Unit name, e.g. "JRG-1"
    public string Name { get; } = name;

    // Unit Location – Used to calculate distances to events
    public Location Location { get; } = location;

    // List of vehicles assigned to a given unit
    private readonly List<Vehicle> _vehicles = [];

    // Adding a vehicle to a unit
    public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);

    // Externally read-only – no one outside the class can interfere with the collection
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    // Counts how many vehicles are available and can be sent into action
    public int FreeVehicleCount() => _vehicles.Count(v => v.State == VehicleState.Free);

    // Clear description for debugging and logging
    public override string ToString() => $"Unit {{Id={Id}, Name={Name}, FreeVehicles={FreeVehicleCount()}}}";
}