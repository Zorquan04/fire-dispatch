using FireDispatch.Models;

namespace FireDispatch.Interfaces;

public interface IObserver
{
    void Update(string message, VehicleState? state = null);
}