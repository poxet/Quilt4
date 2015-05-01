using System;
using System.Collections.Generic;
using System.Linq;
using Quilt4.Interface;

namespace Quilt4.BusinessEntities
{
    public class ApplicationGroup : IApplicationGroup
    {
        private readonly string _name;
        private readonly List<IApplication> _applications;

        public ApplicationGroup(string name, IEnumerable<IApplication> applications)
        {
            _name = name;
            _applications = applications.ToList();
        }

        public string Name { get { return _name; } }
        public IEnumerable<IApplication> Applications { get { return _applications; } }

        public void Add(IApplication application)
        {
            if (_applications.Any(x => string.Compare(x.Name, application.Name, StringComparison.InvariantCultureIgnoreCase) == 0))
                throw new InvalidOperationException("The application name already exists in this group.");

            _applications.Add(application);
        }

        public void Remove(IApplication application)
        {
            if (!_applications.Remove(application))
                throw new InvalidOperationException("Cannot remove application from application group.");
        }
    }
}