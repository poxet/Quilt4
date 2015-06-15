using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Setting : ISetting
    {
        public Setting(string name, string value, string type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }
    }
}