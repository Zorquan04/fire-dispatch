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
    Free, // pojazd dostępny do dysponowania
    Assigned, // zadysponowany (wyjazd w toku)
    Busy // na miejscu działania
}