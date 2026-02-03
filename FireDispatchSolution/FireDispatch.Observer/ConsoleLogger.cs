using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// Simple observer - prints messages to the console
public class ConsoleLogger : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        // If information about the vehicle's condition has arrived, we change the color of the log
        if (state != null)
        {
            Console.ForegroundColor = state switch
            {
                VehicleState.Free => ConsoleColor.Green,
                VehicleState.Assigned => ConsoleColor.Yellow,
                VehicleState.EnRoute => ConsoleColor.Cyan,
                VehicleState.OnScene => ConsoleColor.Magenta,
                VehicleState.Returning => ConsoleColor.DarkCyan,
                _ => ConsoleColor.White
            };
        }
        
        // If a vehicle is specified in the message, the log will handle UnitObserver
        if (vehicle != null) return;
        
        // Writing a message to the log
        Console.WriteLine($"[LOG] {message}");
        Console.ResetColor();
    }
}