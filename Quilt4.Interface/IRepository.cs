﻿using System;
using System.Collections.Generic;

namespace Quilt4.Interface
{
    public interface IRepository
    {
        void AddInitiative(IInitiative initiative);
        void UpdateInitiative(IInitiative initiative);

        IEnumerable<IInitiative> GetInitiativesByDeveloper(string developerName);
        IEnumerable<IApplicationGroup> GetApplicationGroups(Guid initiativeId);
        IEnumerable<IInitiative> GetInitiativeHeadsByDeveloper(string developerName);
        IEnumerable<IInitiative> GetInitiatives();
        IInitiative GetInitiative(Guid initiativeId);
        IInitiative GetInitiativeByClientToken(string clientToken);
        IInitiative GetInitiativeByApplication(Guid applicationId);
        void DeleteInitiative(Guid initiativeId);

        void UpdateApplicationVersion(IApplicationVersion applicationVersion);
        IEnumerable<IApplicationVersion> GetApplicationVersions(Guid applicationId);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForDeveloper(string developerName);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForApplications(IEnumerable<Guid> applicationIds);
        IEnumerable<IApplicationVersion> GetApplicationVersionsForMachine(string machineId);
        void UpdateSessionUsage(Guid sessionId, DateTime serverLastKnown);
        IApplicationVersion GetApplicationVersion(string applicationVersionFingerprint);
        void AddApplicationVersion(IApplicationVersion applicationVersion);
        void DeleteApplicationVersion(string applicationVersionFingerprint);
        IApplicationVersion UpdateApplicationVersionId(string applicationVersionFingerprint, Guid applicationId);

        void AddSession(ISession session);
        ISession GetSession(Guid id);
        IEnumerable<ISession> GetSessionsForApplicationVersion(string applicationVersionId);
        IEnumerable<ISession> GetSessionsForApplication(Guid applicationId);
        IEnumerable<ISession> GetSessionsForDeveloper(string developerName);
        IEnumerable<ISession> GetSessionsForMachine(string machineFingerprint);
        void EndSession(Guid sessionId, DateTime serverEndTime);

        IUser GetUser(string fingerprint);
        void AddUser(IUser user);
        IEnumerable<IUser> GetUsersByApplicationVersion(string applicationFingerprint);

        IMachine GetMachine(string id);
        void AddMachine(IMachine machine);
        IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationFingerprint);

        void RegisterToolkitCompability(Version serverVersion, DateTime registerDate, string supportToolkitNameVersion, ECompatibility compatibility);
        IEnumerable<IToolkitCompatibilities> GetToolkitCompability(Version version);
        IEnumerable<ISession> GetSessionsForApplications(IEnumerable<Guid> initiativeId);
        void UpdateMachine(IMachine machine);
        bool CanConnect();

        IEnumerable<ISession> GetSessionStatistics(DateTime from, DateTime to);
        IEnumerable<IIssue> GetIssueStatistics(DateTime from, DateTime to);
    }
}