using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Fingerprint : IFingerprint
    {
        private readonly string _value;

        private Fingerprint(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new FingerprintException("No value for fingerprint provided. A globally unique identifier should be provided, perhaps a hash from unique data.");
            _value = value;
        }

        public static implicit operator string(Fingerprint item)
        {
            return item._value;
        }

        public static implicit operator Fingerprint(string item)
        {
            return new Fingerprint(item);
        }
    }
}