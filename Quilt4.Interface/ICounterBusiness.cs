using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Interface
{
    public interface ICounterBusiness
    {
        RegisterCounterResponse RegisterCounter(RegisterCounterRequest data);
    }
}