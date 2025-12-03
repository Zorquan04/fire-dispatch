namespace FireDispatch.Models;

/// Reprezentuje pojazd ratowniczy. Pojazdy należą do jednostki (UnitId).
public class Vehicle(string name, Guid unitId)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public VehicleState State { get; set; } = VehicleState.Free; // domyślnie wolny
    public Guid UnitId { get; set; } = unitId; // identyfikator jednostki macierzystej
    
    public override string ToString()
    {
        return $"Vehicle {{Id={Id}, Name={Name}, State={State}}}";
    }
}