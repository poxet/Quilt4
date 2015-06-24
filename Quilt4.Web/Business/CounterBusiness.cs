using Quilt4.Interface;
using Tharga.Quilt4Net.DataTransfer;

namespace Quilt4.Web.Business
{
    public class CounterBusiness : ICounterBusiness
    {
        public RegisterCounterResponse RegisterCounter(RegisterCounterRequest data)
        {
            //TODO: Store counter data
            return new RegisterCounterResponse { };
        }
    }
}