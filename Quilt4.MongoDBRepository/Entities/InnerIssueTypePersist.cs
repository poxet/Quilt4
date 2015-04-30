namespace Quilt4.MongoDBRepository.Entities
{
    internal class InnerIssueTypePersist
    {
        public string ExceptionTypeName { get; internal set; }
        public string Message { get; internal set; }
        public string StackTrace { get; internal set; }
        public string IssueLevel { get; internal set; }
        public InnerIssueTypePersist Inner { get; internal set; }
    }
}