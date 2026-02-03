namespace FireDispatch.Interfaces;

// Interface for the Subject in the Observer Pattern
// Manages the list of observers and notifies them of changes
public interface ISubject
{
    // Adding a follower to your subscription
    void Attach(IObserver observer);

    // Removing a follower
    void Detach(IObserver observer);

    // Notify all followers of the change
    void Notify(string message);
}