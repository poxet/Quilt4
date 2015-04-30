using System;
using System.Collections.Generic;
using Tharga.Quilt4Net.DataTransfer;

namespace Tharga.Quilt4Net.Web
{
    internal static class DynamicExtensions
    {
        public static RegisterIssueRequest ToRegisterIssueRequest(Dictionary<string, object> dataD)
        {
            var data = new RegisterIssueRequest();

            if (dataD.ContainsKey("UserHandle"))
                data.UserHandle = dataD["UserHandle"] as string;

            if (dataD.ContainsKey("UserInput"))
                data.UserInput = dataD["UserInput"] as string;

            if (dataD.ContainsKey("Id"))
            {
                Guid id;
                if (Guid.TryParse(dataD["Id"] as string, out id))
                    data.Id = id;
            }

            if (dataD.ContainsKey("VisibleToUser"))
                data.VisibleToUser = dataD["VisibleToUser"] as bool?;

            if (dataD.ContainsKey("ClientTime"))
            {
                DateTime dlientTime;
                if (DateTime.TryParse(dataD["ClientTime"] as string, out dlientTime))
                    data.ClientTime = dlientTime;
            }

            if (dataD.ContainsKey("IssueThreadGuid"))
            {
                Guid issueThreadGuid;
                if (Guid.TryParse(dataD["IssueThreadGuid"] as string, out issueThreadGuid))
                    data.IssueThreadGuid = issueThreadGuid;
            }

            if (dataD.ContainsKey("Data"))
                data.Data = ToDictionary(dataD["Data"]);

            var issueTypeD = dataD["IssueType"] as dynamic;
            data.IssueType = ToIssueType(issueTypeD);

            var sessionD = dataD["Session"] as dynamic;
            data.Session = DynamicExtensions.ToSession(sessionD);

            return data;
        }

        private static IssueType ToIssueType(Dictionary<string,object> issueTypeD)
        {
            var issueType = new IssueType();

            if (issueTypeD.ContainsKey("ExceptionTypeName"))
                issueType.ExceptionTypeName = issueTypeD["ExceptionTypeName"] as string;

            if (issueTypeD.ContainsKey("IssueLevel"))
                issueType.IssueLevel = issueTypeD["IssueLevel"] as string;

            if (issueTypeD.ContainsKey("Message"))
                issueType.Message = issueTypeD["Message"] as string;

            if (issueTypeD.ContainsKey("StackTrace"))
                issueType.StackTrace = issueTypeD["StackTrace"] as string;

            if (issueTypeD.ContainsKey("Inner"))
            {
                var innerD = issueTypeD["Inner"] as dynamic;
                if (innerD != null)
                    issueType.Inner = ToIssueType(innerD);
            }

            return issueType;
        }

        public static DataTransfer.Session ToSession(Dictionary<string, object> sessionD)
        {
            var session = new DataTransfer.Session();

            try
            {
                if (sessionD.ContainsKey("SessionGuid"))
                {
                    Guid sessionGuid;
                    if (Guid.TryParse(sessionD["SessionGuid"] as string, out sessionGuid))
                        session.SessionGuid = sessionGuid;
                }

                if (sessionD.ContainsKey("ClientStartTime"))
                {
                    DateTime clientStartTime;
                    if (DateTime.TryParse(sessionD["ClientStartTime"] as string, out clientStartTime))
                        session.ClientStartTime = clientStartTime;
                }

                if (sessionD.ContainsKey("ClientToken"))
                {
                    session.ClientToken = sessionD["ClientToken"] as string;
                    if (sessionD.ContainsKey("Environment"))
                        session.Environment = sessionD["Environment"] as string;
                }

                if (sessionD.ContainsKey("Application"))
                {
                    var applicationD = sessionD["Application"] as dynamic;
                    session.Application = ToApplication(applicationD);
                }

                if (sessionD.ContainsKey("Machine"))
                {
                    var machineD = sessionD["Machine"] as dynamic;
                    session.Machine = ToMachine(machineD);
                }

                if (sessionD.ContainsKey("User"))
                {
                    var userD = sessionD["User"] as dynamic;
                    session.User = ToUser(userD);
                }
            }
            catch (Exception exception)
            {
                var exp = new InvalidOperationException("Unable to parse session data part.", exception);
                var data = Issue.Register(exp);
                throw exp.AddData("IssueInstanceTicket", data.IssueInstanceTicket).AddData("IssueTypeTicket", data.IssueTypeTicket);
            }

            return session;
        }

        private static UserData ToUser(Dictionary<string,object> userD)
        {
            var user = new UserData();

            try
            {
                if (userD.ContainsKey("Fingerprint"))
                    user.Fingerprint = userD["Fingerprint"] as string;

                if (userD.ContainsKey("UserName"))
                    user.UserName = userD["UserName"] as string;
            }
            catch (Exception exception)
            {
                var exp = new InvalidOperationException("Unable to parse user data part.", exception);
                var data = Issue.Register(exp);
                throw exp.AddData("IssueInstanceTicket", data.IssueInstanceTicket).AddData("IssueTypeTicket", data.IssueTypeTicket);
            }

            return user;
        }

        private static MachineData ToMachine(Dictionary<string, object> machineD)
        {
            var machine = new MachineData();

            try
            {
                machine.Fingerprint = machineD["Fingerprint"] as string;
                machine.Name = machineD["Name"] as string;

                if (machineD.ContainsKey("Data"))
                    machine.Data = ToDictionary(machineD["Data"]);
                else
                    machine.Data = new Dictionary<string, string>();

                if (!machine.Data.ContainsKey("OsName") && machineD.ContainsKey("OsName"))
                {
                    var osName = machineD["OsName"] as string;
                    if (!string.IsNullOrEmpty(osName))
                        machine.Data.Add("OsName", osName);
                }
            }
            catch (Exception exception)
            {
                var exp = new InvalidOperationException("Unable to parse machine data part.", exception);
                var data = Issue.Register(exp);
                throw exp.AddData("IssueInstanceTicket", data.IssueInstanceTicket).AddData("IssueTypeTicket", data.IssueTypeTicket);
            }

            return machine;
        }

        private static Dictionary<string, string> ToDictionary(dynamic data)
        {
            var d = new Dictionary<string, string>();
            if (data != null)
            {
                foreach (var item in data)
                    d.Add(item.Key, item.Value);
            }
            return d;
        }

        private static ApplicationData ToApplication(Dictionary<string,object> applicationD)
        {
            var application = new ApplicationData();

            try
            {
                if (applicationD.ContainsKey("Fingerprint"))
                    application.Fingerprint = applicationD["Fingerprint"] as string;

                if (applicationD.ContainsKey("Name"))
                    application.Name = applicationD["Name"] as string;

                if (applicationD.ContainsKey("SupportToolkitNameVersion"))
                    application.SupportToolkitNameVersion = applicationD["SupportToolkitNameVersion"] as string;

                if (applicationD.ContainsKey("Version"))
                    application.Version = applicationD["Version"] as string;

                if (applicationD.ContainsKey("BuildTime"))
                {
                    DateTime buildTime;
                    if (DateTime.TryParse(applicationD["BuildTime"] as string, out buildTime))
                        application.BuildTime = buildTime;
                }
            }
            catch (Exception exception)
            {
                var exp = new InvalidOperationException("Unable to parse application data part.", exception);
                var data = Issue.Register(exp);
                throw exp.AddData("IssueInstanceTicket", data.IssueInstanceTicket).AddData("IssueTypeTicket", data.IssueTypeTicket);
            }

            return application;
        }
    }
}