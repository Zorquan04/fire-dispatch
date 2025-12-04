namespace FireDispatch.Models;

// Jednostka straży (JRG/OSP) posiadająca pojazdy i koordynaty
public class Unit(string name, Location location)
{
    // Losowo generowane ID jednostki
    private Guid Id { get; } = Guid.NewGuid();

    // Nazwa jednostki, np. "JRG-1"
    public string Name { get; } = name;

    // Położenie jednostki – używane do liczenia odległości do zdarzeń
    public Location Location { get; } = location;

    // Lista pojazdów przypisana do danej jednostki
    private readonly List<Vehicle> _vehicles = [];

    // Dodanie pojazdu do jednostki
    public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);

    // Tylko do odczytu z zewnątrz – nikt spoza klasy nie może ingerować w kolekcję
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    // Liczy ile pojazdów jest wolnych i może zostać wysłanych do akcji
    public int FreeVehicleCount() => _vehicles.Count(v => v.State == VehicleState.Free);

    // Czytelny opis do debugowania i logów
    public override string ToString() => $"Unit {{Id={Id}, Name={Name}, FreeVehicles={FreeVehicleCount()}}}";
}