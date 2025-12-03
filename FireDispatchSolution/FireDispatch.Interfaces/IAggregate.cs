namespace FireDispatch.Interfaces;

public interface IAggregate<T>
{
    IIterator<T> GetIterator();
}