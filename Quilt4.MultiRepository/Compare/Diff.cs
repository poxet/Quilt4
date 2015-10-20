namespace Quilt4.MultiRepository.Compare
{
    public class Diff : IDiff
    {
        public Diff(string objectName, string otherObjectName, string message)
        {
            ObjectName = objectName ?? "N/A";
            OtherObjectName = otherObjectName ?? "N/A";
            Message = message;
        }

        public string ObjectName { get; private set; }
        public string OtherObjectName { get; private set; }
        public string Message { get; private set; }
    }
}