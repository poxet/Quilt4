using System;
using System.Collections.Generic;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Issue : IIssue
    {
        private readonly Guid _id;
        private readonly DateTime _clientTime;
        private readonly DateTime _serverTime;
        private readonly bool? _visibleToUser;
        private readonly IDictionary<string, string> _data;
        private readonly Guid? _issueThreadGuid;
        private readonly string _userHandle;
        private readonly string _userInput;
        private readonly int _ticket;
        private readonly Guid _sessionId;

        public Issue(Guid id, DateTime clientTime, DateTime serverTime, bool? visibleToUser, IDictionary<string, string> data, Guid? issueThreadGuid, string userHandle, string userInput, int ticket, Guid sessionId)
        {
            _id = id;
            _clientTime = clientTime;
            _serverTime = serverTime;
            _visibleToUser = visibleToUser;
            _data = data;
            _issueThreadGuid = issueThreadGuid;
            _userHandle = userHandle;
            _userInput = userInput;
            _ticket = ticket;
            _sessionId = sessionId;
        }

        public Guid Id { get { return _id; } }
        public DateTime ClientTime { get { return _clientTime; } }
        public DateTime ServerTime { get { return _serverTime; } }
        public bool? VisibleToUser { get { return _visibleToUser; } }
        public IDictionary<string, string> Data { get { return _data; } }
        public Guid? IssueThreadGuid { get { return _issueThreadGuid; } }
        public string UserHandle { get { return _userHandle; } }
        public string UserInput { get { return _userInput; } }
        public int Ticket { get { return _ticket; } }
        public Guid SessionId { get { return _sessionId; } }
    }
}