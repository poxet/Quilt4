using Microsoft.AspNet.Identity;

namespace Quilt4.Interface
{
    public interface IApplicationUser : IUser<string>
    {
        string PasswordHash { get; }
    }
}