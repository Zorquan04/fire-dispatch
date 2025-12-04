using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

// Prosty obserwator - wypisuje komunikaty na konsolę
public class ConsoleLogger : IObserver
{
    public void Update(string message, Vehicle? vehicle = null, VehicleState? state = null)
    {
        // Jeśli przyszła informacja o stanie pojazdu, zmieniamy kolor logu
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
        
        // Jeśli w komunikacie pojazd jest podany, log obsłuży UnitObserver
        if (vehicle != null) return;
        
        // Wypisanie komunikatu w logu
        Console.WriteLine($"[LOG] {message}");
        Console.ResetColor();
    }
}