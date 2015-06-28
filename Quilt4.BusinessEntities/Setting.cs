using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Setting : ISetting
    {
        public Setting(string name, string value, string type, bool encrypted)
        {
            Name = name;
            Value = value;
            Type = type;
            Encrypted = encrypted;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }
        public bool Encrypted { get; private set; }
    }
}