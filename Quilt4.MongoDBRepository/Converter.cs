using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.MongoDBRepository.Entities;

namespace Quilt4.MongoDBRepository
{
    internal static class Converter
    {
        static Converter()
        {
            Mapper.CreateMap<IInitiative, InitiativePersist>();
            Mapper.CreateMap<IDeveloperRole, DeveloperRolePersist>();
            Mapper.CreateMap<IApplicationGroup, ApplicationGroupPersist>();
            Mapper.CreateMap<IApplication, ApplicationPersist>();
            Mapper.CreateMap<ISession, SessionPersist>();
            Mapper.CreateMap<IApplicationVersion, ApplicationVersionPersist>();
            Mapper.CreateMap<IIssueType, IssueTypePersist>();
            Mapper.CreateMap<IIssue, IssuePersist>();
            Mapper.CreateMap<IInnerIssueType, InnerIssueTypePersist>();
            Mapper.CreateMap<IUser, UserPersist>();
            Mapper.CreateMap<IMachine, MachinePersist>();
        }

        public static InitiativePersist ToPersist(this IInitiative item)
        {
            var response = Mapper.Map<IInitiative, InitiativePersist>(item);
            return response;
        }

        public static MachinePersist ToPersist(this IMachine item)
        {
            var response = Mapper.Map<IMachine, MachinePersist>(item);
            return response;
        }

        public static SessionPersist ToPersist(this ISession item)
        {
            var response = Mapper.Map<ISession, SessionPersist>(item);
            return response;
        }

        public static UserPersist ToPersist(this IUser item)
        {
            var response = Mapper.Map<IUser, UserPersist>(item);
            return response;
        }

        public static ApplicationVersionPersist ToPersist(this IApplicationVersion item)
        {
            var response = Mapper.Map<IApplicationVersion, ApplicationVersionPersist>(item);
            return response;
        }

        public static IInitiative ToEntity(this InitiativePersist item)
        {
            if (item == null) return null;
            var response = new Initiative(item.Id, item.Name, item.ClientToken, item.OwnerDeveloperName ?? "N/A", item.DeveloperRoles != null ? item.DeveloperRoles.Select(x => ToEntity((DeveloperRolePersist)x)) : new List<IDeveloperRole>(), item.ApplicationGroups.Select(x => x.ToEntity()));
            return response;
        }

        public static IInitiative ToEntityHead(this InitiativePersist item)
        {
            if (item == null) return null;
            var response = new Initiative(item.Id, item.Name, item.ClientToken, item.OwnerDeveloperName ?? "N/A", new List<IDeveloperRole>(), new List<ApplicationGroup>());
            return response;
        }//item.DeveloperRoles != null ? item.DeveloperRoles.Select(x => x.ToEntity()) :

        public static IDeveloperRole ToEntity(this DeveloperRolePersist item)
        {
            return new DeveloperRole(item.DeveloperName, item.RoleName, item.InviteCode, item.InviteEMail, item.InviteTime);
        }

        public static IApplicationGroup ToEntity(this ApplicationGroupPersist item)
        {
            return new ApplicationGroup(item.Name, item.Applications.Select(x => x.ToEntity()));
        }

        public static IApplication ToEntity(this ApplicationPersist item)
        {
            return new Application(item.Id, item.Name, item.FirstRegistered, item.TicketPrefix);
        }

        public static ISession ToEntity(this SessionPersist item)
        {
            if (item == null) return null;
            var response = new Session(item.Id, item.ApplicationVersionId, item.Environment, item.ApplicationId, item.MachineFingerprint, item.UserFingerprint, item.ClientStartTime, item.ServerStartTime, item.ServerEndTime, item.ServerLastKnown, item.CallerIp);
            return response;
        }

        public static IUser ToEntity(this UserPersist item)
        {
            if (item == null) return null;
            return new User(item.Id, item.UserName);
        }

        public static IMachine ToEntity(this MachinePersist item)
        {
            if (item == null) return null;

            var dict = new Dictionary<string, string>();
            if (item.Data != null)
                dict = item.Data.ToDictionary(x => x.Key, x => x.Value);

            var machine = new Machine(item.Id, item.Name, dict);
            return machine;
        }

        public static IApplicationVersion ToEntity(this ApplicationVersionPersist item)
        {
            if (item == null) return null;
            var response = new ApplicationVersion(item.Id, item.ApplicationId, item.Version, item.IssueTypes.Select(x => x.ToEntity()), item.ResponseMessage, item.IsOfficial, item.Ignore, item.SupportToolkitNameVersion, item.BuildTime);
            return response;
        }

        public static IIssueType ToEntity(this IssueTypePersist item)
        {
            IssueLevel issueLevel;
            var replace = item.IssueLevel.Replace("Message", "").Replace("Exception", "");

            if (replace == "")
                replace = "Error";

            if (!Enum.TryParse(replace, true, out issueLevel))
                throw new InvalidOperationException(string.Format("Cannot parse {0} to IssueLevel.", item.IssueLevel));

            return new IssueType(item.ExceptionTypeName, item.Message, item.StackTrace, issueLevel, item.Inner.ToEntity(), item.Issues.Select(x => x.ToEntity()), item.Ticket, item.ResponseMessage);
        }

        public static IInnerIssueType ToEntity(this InnerIssueTypePersist item)
        {
            if (item == null) return null;
            return new InnerIssueType(item.ExceptionTypeName, item.Message, item.StackTrace, item.IssueLevel, item.Inner.ToEntity());
        }

        public static IIssue ToEntity(this IssuePersist item)
        {
            return new Issue(item.Id, item.ClientTime, item.ServerTime, item.VisibleToUser, item.Data, item.IssueThreadGuid, item.UserHandle, item.UserInput, item.Ticket, item.SessionId);
        }

        public static IToolkitCompatibilities ToEntity(this ToolkitCompabilityPersist item)
        {
            return new ToolkitCompability(new Version(item.ServerVersion), item.RegisterDate, item.SupportToolkitNameVersion, (ECompatibility)item.Compatibility, null);
        }
    }
}