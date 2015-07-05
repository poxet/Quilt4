using Quilt4.Interface;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class DataBaseInfoEntity : IDataBaseInfo
    {
        public bool Online { get; internal set; }
        public string Server { get; internal set; }
        public string Name { get; internal set; }
    }
}