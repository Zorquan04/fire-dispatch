using FireDispatch.Models;
using FireDispatch.Interfaces;
using FireDispatch.Collections;
using FireDispatch.Strategy;
using System.Collections.Concurrent;
using FireDispatch.Observer;

namespace FireDispatch.Simulation;

// Class responsible for all event simulation logic.
// Handles new reports, assigns vehicles, simulates arrival, action, and return to the fire station.
// Implements IObserver to forward logs to other observers.
public class EventSimulator(Dispatcher dispatcher, CommandCenter commandCenter) : IObserver
{
    // List of simulator observers (e.g. logger, unit observer)
    private readonly List<IObserver> _observers = new();

    // Queue of events waiting for available vehicles
    private readonly ConcurrentQueue<Event> _pendingEvents = new();

    // Simulation statistics
    private int _totalEvents;
    private int _totalVehiclesDispatched;
    private int _totalTimeMs;

    // Observer registration (Logger, UnitObserver)
    public void Attach(IObserver observer) => _observers.Add(observer);
    public void Detach(IObserver observer) => _observers.Remove(observer);

    // Forwarding the message to all observers
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        foreach (var obs in _observers.ToArray())
            obs.Update(message, vehicle, state);
    }

    // The main method of handling the request - run asynchronously
    public async Task HandleEventAsync(Event evt)
    {
        var rand = new Random();

        // Delaying the start to make the simulation look more natural
        await Task.Delay(rand.Next(200, 500));

        // We inform the command center about a new event
        commandCenter.NewEvent(evt);

        var eventStartTime = DateTime.Now;
        Interlocked.Increment(ref _totalEvents);

        Update($"--- NEW REPORT: {evt.Label} ---");
        Update($"Location: {evt.Location.Latitude:F5}, {evt.Location.Longitude:F5}");

        // Number of vehicles required depending on the type of application
        int requiredCount = evt.Type switch
        {
            EventType.Pz => 3, // Fire → stronger forces
            EventType.Mz => 2, // Local threat → standard
            EventType.Af => 0, // False alarm → fast comeback
            _ => 2
        };

        // We are trying to ship vehicles
        var vehicles = dispatcher.Dispatch(evt, requiredCount).ToList();

        // If there are no free times, we postpone them until later
        if (!vehicles.Any())
        {
            Update("No available vehicles - adding the event to the waiting queue");
            _pendingEvents.Enqueue(evt);
            return;
        }

        Interlocked.Add(ref _totalVehiclesDispatched, vehicles.Count);

        // We create a collection of vehicles with iterators
        var vehicleCollection = new VehicleCollection(vehicles);

        // 1) Vehicle assignment
        var iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Assign();
            Update($"Vehicle {v.Name} assigned to an report {evt.Label}", v, v.State);
        }

        // 2) Departure and arrival at the venue
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.StartTravel();
            Update($"Vehicle {v.Name} on the way to the report {evt.Label}", v, v.State);

            int travelTimeMs = rand.Next(1000, 4000);
            Update($"Vehicle arrival time {v.Name}: {travelTimeMs / 1000.0:F1}s");
            await Task.Delay(travelTimeMs);
        }

        // Random chance of false alarm (here 5%)
        bool falseAlarm = rand.Next(100) < 5;

        // 3) Actions at the scene
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Arrive();
            Update($"Vehicle {v.Name} at the scene {evt.Label}", v, v.State);

            // If false – we turn back without action
            if (falseAlarm)
            {
                Update($"FALSE ALARM at {evt.Label}! The units are turning back.", v, v.State);
                v.Return();
                Update($"Vehicle {v.Name} returns to unit (false alarm)", v, v.State);
                continue;
            }

            // Normal action
            int actionTimeMs = rand.Next(5000, 15000);
            Update($"Vehicle activity time {v.Name}: {actionTimeMs / 1000.0:F1}s");
            await Task.Delay(actionTimeMs);
        }

        // 4) Return to base
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();

            if (!falseAlarm)
            {
                v.Return();
                Update($"Vehicle {v.Name} returns to the unit", v, v.State);
            }

            int returnTimeMs = rand.Next(1000, 4000);
            Update($"Vehicle return time {v.Name}: {returnTimeMs / 1000.0:F1}s");
            await Task.Delay(returnTimeMs);
        }

        // 5) Release of vehicles upon return
        iterator = vehicleCollection.GetIterator();
        while (iterator.HasNext())
        {
            var v = iterator.Next();
            v.Free();
            Update($"Vehicle {v.Name} available again", v, v.State);
        }

        // Recording the time of the entire event into statistics
        _totalTimeMs += (int)(DateTime.Now - eventStartTime).TotalMilliseconds;

        // Check if there are any other applications waiting in the queue
        await CheckPendingEventsAsync();
    }

    // Handling subsequent pending events if vehicles become available
    private Task CheckPendingEventsAsync()
    {
        if (_pendingEvents.TryDequeue(out var nextEvent))
            _ = Task.Run(() => HandleEventAsync(nextEvent));

        return Task.CompletedTask;
    }

    // Simple printout of final statistics
    public void PrintStatistics()
    {
        Console.WriteLine("\n--- SIMULATION STATISTICS ---");
        Console.WriteLine($"Number of reports handled: {_totalEvents}");
        Console.WriteLine($"Total number of departing vehicles: {_totalVehiclesDispatched}");
        Console.WriteLine($"Total time (ms) of actions and arrivals: {_totalTimeMs}");
        Console.WriteLine($"Average time per report (ms): {(_totalEvents > 0 ? _totalTimeMs / _totalEvents : 0)}");
        Console.WriteLine("----------------------------\n");
    }
}