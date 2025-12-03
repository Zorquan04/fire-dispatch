namespace FireDispatch.Interfaces;

public interface IIterator<T>
{
    bool HasNext();
    T Next();
}