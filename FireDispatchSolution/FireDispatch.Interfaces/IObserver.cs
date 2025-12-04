using FireDispatch.Models;

namespace FireDispatch.Interfaces;

public interface IObserver
{
    void Update(string message, Vehicle? vehicle = null, VehicleState? state = null);
}