namespace Quilt4.Interface
{
    public interface ISetting
    {
        string Name { get; }
        string Value { get; }
        string Type { get; }
        bool Encrypted { get; }
    }
}