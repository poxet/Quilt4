using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface ICounter
    {
        string CounterName { get; } //(Ex. Application, IssueType, Logon)
        DateTime DateTime { get; } //(UTC time for the action)
        decimal? Duration { get; } //(null, value of any optional unit)
        int Count { get; } //(1, or a larger number if aggregated, or -1 if decreasing, ie DELETE)
        Dictionary<string,string> Path { get; } //(Used to specify where the data belongs. Ex. { Initiative = 'ABC', Application='DEF', Version='1.2.3.4' })
        Dictionary<string, string> Data { get; } //(Used for any additional information. Ex. { Blah = 'a1', Blah2 = 'b2' })
        string Level { get; } //(null, Information, Warning, Error)
        string Environment { get; } //(null, Name of the environment)
    }
}