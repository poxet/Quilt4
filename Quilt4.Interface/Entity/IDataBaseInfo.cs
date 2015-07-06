namespace Quilt4.Interface
{
    public interface IDataBaseInfo
    {
        bool Online { get; }
        string Server { get; }
        string Name { get; }
    }
}