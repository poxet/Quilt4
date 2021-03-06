using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class Initiative : IInitiative
    {
        private readonly Guid _id;
        private readonly string _clientToken;
        private readonly List<IDeveloperRole> _developerRoles;
        private readonly List<IApplicationGroup> _applicationGroups;
        private string _name;
        private string _ownerDeveloperName;

        public Initiative(Guid id, string name, string clientToken, string ownerDeveloperName, IEnumerable<IDeveloperRole> developerRoles, IEnumerable<IApplicationGroup> applicationGroups)
        {
            _id = id;
            Name = name;
            _clientToken = clientToken;
            OwnerDeveloperName = ownerDeveloperName;
            _applicationGroups = applicationGroups.ToList();
            _developerRoles = developerRoles.ToList();
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return !string.IsNullOrEmpty(_name) ? _name : null; } set { _name = value; } }
        public string ClientToken { get { return _clientToken; } }
        public string OwnerDeveloperName
        {
            get { return _ownerDeveloperName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException(string.Format("Initiative needs to have a owner developer to be saved."));

                _ownerDeveloperName = value;
            }
        }
        public IEnumerable<IDeveloperRole> DeveloperRoles { get { return _developerRoles; } }
        public IEnumerable<IApplicationGroup> ApplicationGroups { get { return _applicationGroups; } }

        public void AddApplicationGroup(IApplicationGroup applicationGroup)
        {
            if (_applicationGroups.Any(x => string.Compare(x.Name, applicationGroup.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                throw new InvalidOperationException("There is already an application group with this name in this initiative.");

            _applicationGroups.Add(applicationGroup);
        }

        public void RemoveApplicationGroup(IApplicationGroup applicationGroup)
        {
            if (!_applicationGroups.Remove(applicationGroup))
                throw new InvalidOperationException("Cannot remove application group from initiative.");
        }

        public string AddDeveloperRolesInvitation(string email)
        {
            var inviteCode = RandomUtility.GetRandomString(10);
            _developerRoles.Add(new DeveloperRole(null, RoleNameConstants.Invited, inviteCode, email, DateTime.UtcNow, new DateTime(01, 01, 01, 01, 01, 01)));
            return inviteCode;
        }

        public void RemoveDeveloperRole(string developer)
        {
            var item = _developerRoles.FirstOrDefault(x => string.Compare(x.DeveloperName, developer, StringComparison.InvariantCultureIgnoreCase) == 0);
            if (item == null)
                item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteEMail, developer, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (item != null)
                _developerRoles.Remove(item);
        }

        //public void DeclineInvitation(string inviteCode)
        //{
        //    var item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteCode, inviteCode, StringComparison.InvariantCultureIgnoreCase) == 0);
        //    if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation with provided code."));
        //    item.RoleName = RoleNameConstants.Declined;
        //}

        //public void ConfirmInvitation(string inviteCode, string developerName)
        //{
        //    var item = _developerRoles.FirstOrDefault(x => string.Compare(x.InviteCode, inviteCode, StringComparison.InvariantCultureIgnoreCase) == 0);
        //    if (item == null) throw new NullReferenceException(string.Format("Cannot find invitation with provided code."));
        //    item.DeveloperName = developerName;
        //    item.RoleName = RoleNameConstants.Administrator;
        //    item.InviteResponseTime = DateTime.Now;
        //}
    }
}