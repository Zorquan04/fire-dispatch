using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Simulation;

namespace FireDispatch.Observer;

public class UnitObserver(Unit unit, EventSimulator simulator, IObserver logger) : IObserver
{
    public void Update(string message, VehicleState? state = null)
    {
        logger.Update($"[{unit.Name}] {message}", state);

        // Generowanie zdarzenia przy lokalizacji jednostki
        var evt = new Event(EventType.Mz, unit.Location); // demo: typ można losować

        // Wywołanie równoległe w EventSimulator
        _ = Task.Run(() => simulator.HandleEventAsync(evt));
    }
}