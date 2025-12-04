using FireDispatch.Models;

namespace FireDispatch.Interfaces;

// Interfejs obserwatora we wzorcu Observer
// Obiekty implementujące go mogą reagować na powiadomienia
public interface IObserver
{
    // Update wywoływany przy Notify()
    // message – komunikat o zdarzeniu
    // vehicle – opcjonalna referencja pojazdu (np. zmiana stanu)
    // state – nowy stan pojazdu
    void Update(string message, Vehicle? vehicle = null, VehicleState? state = null);
}