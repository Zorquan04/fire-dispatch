namespace FireDispatch.Interfaces;

// Iterator interface – defines how to traverse a collection
public interface IIterator<out T>
{
    // Is there another element?
    bool HasNext();
    // Gets the next element in the collection
    T Next();
}