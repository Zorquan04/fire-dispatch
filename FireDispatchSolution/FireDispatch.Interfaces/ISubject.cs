namespace FireDispatch.Interfaces;

// Interfejs obiektu nadzorującego (Subject) w Observer Pattern
// Zarządza listą obserwatorów i powiadamia ich o zmianach
public interface ISubject
{
    // Dodanie obserwatora do subskrypcji
    void Attach(IObserver observer);

    // Usunięcie obserwatora
    void Detach(IObserver observer);

    // Powiadom wszystkich obserwujących o zmianie
    void Notify(string message);
}