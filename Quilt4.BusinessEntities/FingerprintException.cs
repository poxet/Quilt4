using System;

namespace Quilt4.BusinessEntities
{
    public class FingerprintException : Exception
    {
        public FingerprintException(string message)
            : base(message)
        {
        }
    }
}