using System;

namespace Quilt4.MongoDBRepository.Entities
{
    internal class ApplicationPersist
    {
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public DateTime FirstRegistered { get; internal set; }
        public string TicketPrefix { get; internal set; }
        public string DevColor { get; internal set; }
        public string ProdColor { get; internal set; }
        public string CiColor { get; internal set; }
    }
}