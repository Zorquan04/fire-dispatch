using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// Prosty logger do konsoli
public class ConsoleLogger : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
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
        
        if (vehicle != null) return;
        
        Console.WriteLine($"[LOG] {message}");
        Console.ResetColor();
    }
}