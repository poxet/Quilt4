using System;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Application : IApplication
    {
        private readonly Guid _id;
        private readonly string _name;
        private readonly DateTime _firstRegistered;

        public Application(Guid id, string name, DateTime firstRegistered, string ticketPrefix)
        {
            _id = id;
            _name = name;
            _firstRegistered = firstRegistered;
            TicketPrefix = ticketPrefix;
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return _name; } }
        public DateTime FirstRegistered { get { return _firstRegistered; } }
        public string TicketPrefix { get; set; }
    }
}