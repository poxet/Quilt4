namespace Quilt4.Interface
{
    public interface IApplicationUser
    {
        string Id { get; }
        string PasswordHash { get; }
    }
}