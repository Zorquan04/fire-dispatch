namespace FireDispatch.Interfaces;

// Interfejs iteratora – definiuje sposób przechodzenia po kolekcji
public interface IIterator<out T>
{
    // Czy jest kolejny element?
    bool HasNext();
    // Pobiera następny element kolekcji
    T Next();
}