namespace FireDispatch.Models;

/// Jednostka ratownicza (stacja) zawierająca listę pojazdów i położenie.
public class Unit(string name, Location location)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public Location Location { get; set; } = location;
    public List<Vehicle> Vehicles { get; } = new();
    
    // Liczba wolnych pojazdów w jednostce
    public int FreeVehicleCount() => Vehicles.Count(v => v.State == VehicleState.Free);
    
    public override string ToString()
    {
        return $"Unit {{Id={Id}, Name={Name}, FreeVehicles={FreeVehicleCount()}}}";
    }
}