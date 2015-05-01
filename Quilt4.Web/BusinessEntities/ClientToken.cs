using System;
using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    public class ClientToken : IClientToken
    {
        private readonly string _value;

        private ClientToken(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value", "No value for client token provided.");

            _value = value;
        }

        public static implicit operator string(ClientToken item)
        {
            return item._value;
        }

        public static implicit operator ClientToken(string item)
        {
            return new ClientToken(item);
        }
    }
}