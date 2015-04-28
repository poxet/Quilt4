using Microsoft.AspNet.Identity;

namespace Quilt4.Interface
{
    public interface IApplicationUser : IUser<string>
    {
        //string Id { get; }
        string PasswordHash { get; }
    }
}