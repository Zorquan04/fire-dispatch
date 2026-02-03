namespace FireDispatch.Models;

// Event type: PZ (fire), AF (false alarm), MZ (local threat)
public enum EventType
{
    Pz,
    Af,
    Mz
}

// Vehicle Status - simple labels, we will expand later
public enum VehicleState
{
    Free,       // vehicle available
    Assigned,   // assigned to the incident, but not yet en route
    EnRoute,    // en route to the incident
    OnScene,    // at the incident
    Returning   // returning to the unit
}