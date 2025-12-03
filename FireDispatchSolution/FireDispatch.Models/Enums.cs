namespace FireDispatch.Models;

/// Typ zdarzenia: PZ (pożar), AF (alarm fałszywy), MZ (miejscowe zagrożenie)
public enum EventType
{
    Pz,
    Af,
    Mz
}

/// Stan pojazdu — proste etykiety, będziemy rozszerzać później
public enum VehicleState
{
    Free,       // pojazd dostępny
    Assigned,   // przydzielony do zdarzenia, ale jeszcze nie w drodze
    EnRoute,    // w drodze na miejsce zdarzenia
    OnScene,    // na miejscu zdarzenia
    Returning   // wraca do jednostki
}