using System;

namespace Quilt4.Interface
{
    public interface IApplication
    {
        //TODO: REFACTOR: Make immutable
        Guid Id { get; }
        string Name { get; }
        DateTime FirstRegistered { get; }
        string TicketPrefix { get; set; }
        string DevColor { get; set; }
        string ProdColor { get; set; }
        string CiColor { get; set; }
    }
}