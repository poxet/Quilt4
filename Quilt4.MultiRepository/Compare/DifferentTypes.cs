using System;

namespace Quilt4.MultiRepository.Compare
{
    public class DifferentTypes : IDiff
    {
        public DifferentTypes(string objectName, Type type, Type otherType)
        {
            ObjectName = objectName;
            Message = string.Format("The types differs. One type is {0} and the other is {1} in object {2}.", type, otherType, objectName);
        }

        public string Message { get; private set; }
        public string ObjectName { get; private set; }
    }
}