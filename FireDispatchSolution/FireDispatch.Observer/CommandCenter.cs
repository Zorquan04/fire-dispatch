using FireDispatch.Interfaces;

namespace FireDispatch.Observer;

// SKKM – nadzoruje jednostki PSP, powiadamia o nowych zdarzeniach
public class CommandCenter : ISubject
{
    private readonly List<IObserver> _observers = new();

    public void Attach(IObserver observer) => _observers.Add(observer);

    public void Detach(IObserver observer) => _observers.Remove(observer);

    public void Notify(string message)
    {
        foreach (var obs in _observers)
            obs.Update(message);
    }
    
    // Metoda wywołująca zdarzenie i powiadamiająca jednostki
    public void NewEvent(string eventInfo)
    {
        Console.WriteLine($"[SKKM] Nowe zdarzenie: {eventInfo}");
        Notify(eventInfo);
    }
}