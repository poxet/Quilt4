using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Machine : IMachine
    {
        private readonly string _fingerprint;
        private readonly string _name;
        private IDictionary<string, string> _data;

        public Machine(string fingerprint, string name, IDictionary<string, string> data)
        {
            _fingerprint = fingerprint;
            _name = name;
            _data = data;
        }

        public string Id { get { return _fingerprint; } }
        public string Name { get { return _name; } }

        public IDictionary<string, string> Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}