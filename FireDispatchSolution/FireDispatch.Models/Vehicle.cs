namespace FireDispatch.Models;

// Represents a rescue vehicle. Vehicles belong to a unit (UnitId).
public class Vehicle(string name, Unit unit)
{
    private Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public VehicleState State { get; private set; } = VehicleState.Free;
    public Unit Unit { get; } = unit;

    // Vehicle status change - event assignment
    public void Assign() => State = VehicleState.Assigned;

    // Vehicle on the way
    public void StartTravel() => State = VehicleState.EnRoute;

    // The vehicle arrived at the site
    public void Arrive() => State = VehicleState.OnScene;

    // The vehicle is returning
    public void Return() => State = VehicleState.Returning;

    // Vehicle available upon return
    public void Free() => State = VehicleState.Free;

    public override string ToString() => $"Vehicle {{Id={Id}, Name={Name}, State={State}}}";
}