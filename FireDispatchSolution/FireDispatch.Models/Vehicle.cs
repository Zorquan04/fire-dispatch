namespace FireDispatch.Models;

// Reprezentuje pojazd ratowniczy. Pojazdy należą do jednostki (UnitId).
public class Vehicle(string name, Guid unitId)
{
    private Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public VehicleState State { get; private set; } = VehicleState.Free;
    public Guid UnitId { get; set; } = unitId;

    // Zmiana stanu pojazdu — przypisanie do zdarzenia
    public void Assign() => State = VehicleState.Assigned;

    // Pojazd w drodze
    public void StartTravel() => State = VehicleState.EnRoute;

    // Pojazd dotarł na miejsce
    public void Arrive() => State = VehicleState.OnScene;

    // Pojazd wraca
    public void Return() => State = VehicleState.Returning;

    // Pojazd dostępny po powrocie
    public void Free() => State = VehicleState.Free;

    public override string ToString() => $"Vehicle {{Id={Id}, Name={Name}, State={State}}}";
}