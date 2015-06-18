namespace Quilt4.Interface
{
    public interface ICounterLine
    {
        string Key { get; }
        int[] Counts { get; }
    }
}