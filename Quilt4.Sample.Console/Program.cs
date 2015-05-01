using Tharga.Quilt4Net;

namespace Quilt4.Sample.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(2000);

            Configuration.ClientToken = "???";
            Configuration.Target.Location = "http://localhost:54942/";

            Session.Register();
            Issue.Register("abc", Issue.MessageIssueLevel.Information);
            Session.End();

            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }
    }
}