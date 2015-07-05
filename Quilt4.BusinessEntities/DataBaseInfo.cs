using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class DataBaseInfo : IDataBaseInfo
    {
        //TODO: REFACTOR: Make immutable
        public bool Online { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
    }
}