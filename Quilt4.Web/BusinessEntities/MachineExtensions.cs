using Quilt4.Interface;

namespace Quilt4.Web.BusinessEntities
{
    static class MachineExtensions
    {
        public static bool AreEqual(this IMachine item, IMachine other)
        {
            if (item.Data == null || other.Data == null)
            {
                if (item.Data != null)
                    return false;

                if (other.Data != null)
                    return false;

                return true;
            }

            if (item.Data.Count != other.Data.Count)
                return false;

            foreach (var a in item.Data)
            {
                if (other.Data[a.Key] != a.Value)
                    return false;
            }

            return true;
        }
    }
}