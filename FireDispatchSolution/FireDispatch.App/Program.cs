using FireDispatch.Collections;
using FireDispatch.Models;
using FireDispatch.Observer;
using FireDispatch.Simulation;
using FireDispatch.Strategy;

namespace FireDispatch.App;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("FireDispatch - random event simulation\n");

        // We create fire brigade units
        // We accept base locations as GPS coordinates
        var unit1 = new Unit("JRG-1", new Location(50.05, 19.94));
        var unit2 = new Unit("JRG-2", new Location(50.04, 19.92));

        // Add vehicles to each unit (5 per unit)
        // Dynamically generated name, e.g., JRG1-V1, JRG1-V2...
        for (int i = 1; i <= 5; i++)
        {
            unit1.AddVehicle(new Vehicle($"JRG1-V{i}", unit1));
            unit2.AddVehicle(new Vehicle($"JRG2-V{i}", unit2));
        }

        // Unit Collection – Iterated later by the Vehicle Selection Strategy   
        var unitCollection = new UnitCollection();
        unitCollection.Add(unit1);
        unitCollection.Add(unit2);

        // The dispatcher is responsible for selecting units/vehicles for the event.
        // We inject the strategy – here, the closest vehicle is the priority.
        var dispatcher = new Dispatcher(new NearestFirstStrategy(), unitCollection);

        // SKKM – command center (Mediator/Observer Subject)
        var commandCenter = new CommandCenter();

        // Event simulator – the main orchestrator of the action
        var simulator = new EventSimulator(dispatcher, commandCenter);
        
        // Console logger – displays all messages in the application
        var consoleLogger = new ConsoleLogger();
        simulator.Attach(consoleLogger);
        commandCenter.Attach(consoleLogger);

        // We connect unit observers – their task is to log vehicle activities
        simulator.Attach(new UnitObserver(unit1, consoleLogger));
        simulator.Attach(new UnitObserver(unit2, consoleLogger));
        commandCenter.Attach(new UnitObserver(unit1, consoleLogger));
        commandCenter.Attach(new UnitObserver(unit2, consoleLogger));

        // Random generator + number of events in the simulation
        var rng = new Random();
        int eventCount = 10;

        // We generate events at random intervals (simulation of reality)
        for (int i = 0; i < eventCount; i++)
        {
            // In 70% of cases MZ, 30% PZ
            EventType type = rng.NextDouble() < 0.7 ? EventType.Mz : EventType.Pz;

            // Drawing the location of the event within the city
            double lat = 50.04 + rng.NextDouble() * 0.02;
            double lon = 19.92 + rng.NextDouble() * 0.02;

            // We create a report and send it to the simulator asynchronously
            var evt = new Event(type, new Location(lat, lon));
            _ = simulator.HandleEventAsync(evt);

            // A short break between entries
            await Task.Delay(rng.Next(1000, 3000));
        }

        // We wait until all actions are completed
        await Task.Delay(30000);

        // We print out statistics from the simulation (time, number of events, etc.)
        simulator.PrintStatistics();
        simulator.Detach(consoleLogger);

        Console.WriteLine("Simulation completed");
    }
}