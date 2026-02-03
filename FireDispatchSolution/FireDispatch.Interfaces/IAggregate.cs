namespace FireDispatch.Interfaces;

// Interface for a collection supporting Iterator Pattern
// out T = covariance -> allows you to return a type that inherits from T
public interface IAggregate<out T>
{
    // Creates and returns an iterator that supports traversing a collection.
    IIterator<T> GetIterator();
}