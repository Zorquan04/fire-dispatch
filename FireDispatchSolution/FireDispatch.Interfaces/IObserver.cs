using FireDispatch.Models;

namespace FireDispatch.Interfaces;

// Observer interface in the Observer pattern
// Objects implementing it can respond to notifications
public interface IObserver
{
    // Update called by Notify()
    // message – event message
    // vehicle – optional vehicle reference (e.g., state change)
    // state – new vehicle state
    void Update(string message, Vehicle? vehicle = null, VehicleState? state = null);
}