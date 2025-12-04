namespace FireDispatch.Interfaces;

// Interfejs kolekcji wspierającej Iterator Pattern
// out T = covariance -> pozwala zwracać typ dziedziczący po T
public interface IAggregate<out T>
{
    // Tworzy i zwraca iterator obsługujący poruszanie się po kolekcji
    IIterator<T> GetIterator();
}