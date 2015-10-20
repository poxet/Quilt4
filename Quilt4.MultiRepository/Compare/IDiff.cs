namespace Quilt4.MultiRepository.Compare
{
    public interface IDiff
    {
        string Message { get; }
        string ObjectName { get; }
    }
}