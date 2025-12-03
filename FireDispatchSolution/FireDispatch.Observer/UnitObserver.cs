using FireDispatch.Interfaces;
using FireDispatch.Models;

namespace FireDispatch.Observer;

/// Observer jednostki PSP – reaguje na powiadomienia SKKM
public class UnitObserver(Unit unit) : IObserver
{
    public void Update(string message)
    {
        // Prosty log do konsoli — w przyszłości można tu wywołać strategię dysponowania
        Console.WriteLine($"[{unit.Name}] Otrzymano powiadomienie: {message}");
    }
}