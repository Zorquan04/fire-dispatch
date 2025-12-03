namespace FireDispatch.Interfaces;

public interface IAggregate<out T>
{
    IIterator<T> GetIterator();
}