using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// Centrum dowodzenia (SKKM) pełni funkcję Subject w wzorcu Observer
// Odbiera nowe zgłoszenia i rozsyła informacje do wszystkich obserwatorów (loggerów, unitów itp.)
public class CommandCenter : ISubject
{
    // Lista podłączonych obserwatorów (np. logger, UnitObserver)
    private readonly List<IObserver> _observers = new();

    // Dodanie obserwatora
    public void Attach(IObserver observer) => _observers.Add(observer);

    // Usunięcie obserwatora
    public void Detach(IObserver observer) => _observers.Remove(observer);

    // Wysyłanie komunikatu do wszystkich obserwatorów
    public void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update("[SKKM] " + message);
    }

    // Wywoływane, gdy pojawi się nowe zdarzenie w systemie
    public void NewEvent(Event evt)
    {
        // Informacja na konsoli o przyjęciu zgłoszenia
        Console.WriteLine($"[SKKM] Nowe zgłoszenie: {evt.Label} | {evt.Type} | ({evt.Location.Latitude:F5}, {evt.Location.Longitude:F5})");

        // Rozesłanie informacji do obserwatorów
        Notify($"Zgłoszenie przyjęte: {evt.Label}");
    }
}