namespace FireDispatch.Interfaces;

public interface IIterator<out T>
{
    bool HasNext();
    T Next();
}