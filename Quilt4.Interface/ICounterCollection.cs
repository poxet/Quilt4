namespace Quilt4.Interface
{
    public interface ICounterCollection
    {
        string[] Names { get; }
        ICounterLine[] Lines { get; }
    }
}