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
        int? KeepLatestVersions { get; set; }
    }
}