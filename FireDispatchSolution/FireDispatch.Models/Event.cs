namespace FireDispatch.Models;

// Model pojedynczego zgłoszenia przychodzącego do systemu
// Posiada typ zdarzenia (np. pożar / miejscowe zagrożenie) oraz lokalizację
public class Event(EventType type, Location location)
{
    // Globalny licznik zdarzeń – każde nowe zgłoszenie dostaje kolejny numer
    private static int _counter;

    // Unikalne ID zdarzenia nadawane automatycznie w sposób atomowy (bezpieczny dla wielu wątków)
    private int Id { get; } = Interlocked.Increment(ref _counter);

    // Rodzaj zdarzenia (Enum)
    public EventType Type { get; } = type;

    // Miejsce wystąpienia
    public Location Location { get; } = location;

    // Etykieta pomocnicza np. "Pz-4" – dobre do logów/identyfikacji zdarzeń
    public string Label => $"{Type}-{Id}";
}