using System;
using System.Collections.Generic;
using Tharga.Quilt4Net;

namespace Quilt4.Sample.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                System.Threading.Thread.Sleep(2000);

                Configuration.ClientToken = "EGP67U898AEN034HK11YE8TFN8LLZ5NV"; //TODO: Enter the clientToken you have in your system here.
                Configuration.Target.Location = "http://localhost:54942/";

                Session.Register();

                //Register messages
                var a = Issue.Register("abc", Issue.MessageIssueLevel.Information);
                Issue.Register("abc?", Issue.MessageIssueLevel.Warning, null, null, null, new Dictionary<string, string> { { "D1", "V1" }, { "D2", "V2" } });
                Issue.Register("abc!", Issue.MessageIssueLevel.Error);

                //Register exceptions
                try
                {
                    try
                    {
                        throw new InvalidOperationException("Some exception");
                    }
                    catch (Exception exception)
                    {
                        Issue.Register(exception);
                        throw;
                    }                    
                }
                catch (Exception exception)
                {
                    exception.AddData("Data1", "Value1");
                    exception.AddData("Data2", "Value2");
                    Issue.Register(exception, Issue.ExceptionIssueLevel.Error, false, null, null);
                }

                Session.End();

                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
                System.Console.ReadKey();
            }
        }
    }
}