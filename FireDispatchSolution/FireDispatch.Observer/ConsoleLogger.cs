using FireDispatch.Interfaces;

namespace FireDispatch.Observer;

// Prosty logger do konsoli
public class ConsoleLogger : IObserver
{
    public void Update(string message) => Console.WriteLine($"[LOG] {message}");
}