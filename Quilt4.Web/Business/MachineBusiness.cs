using System.Collections.Generic;
using System.Linq;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.BusinessEntities;

namespace Quilt4.Web.Business
{
    public class MachineBusiness : IMachineBusiness
    {
        private readonly IRepository _repository;

        public MachineBusiness(IRepository repository)
        {
            _repository = repository;
        }

        public void RegisterMachine(IFingerprint id, string name, IDictionary<string, string> data)
        {
            var machine = _repository.GetMachine((Fingerprint)id);
            if (machine == null)
                _repository.AddMachine(new Machine((Fingerprint)id, name, data));
            else
            {
                var newMachine = new Machine((Fingerprint)id, machine.Name, data);
                if (!machine.AreEqual(newMachine))
                {
                    _repository.UpdateMachine(newMachine);
                }
            }
        }

        public IMachine GetMachine(string machineFingerprint)
        {
            return _repository.GetMachine(machineFingerprint);
        }

        public IEnumerable<IMachine> GetMachinesByApplicationVersion(string applicationVersionId)
        {
            var machines = _repository.GetMachinesByApplicationVersion(applicationVersionId).OrderBy(x => x.Name);
            return machines;
        }

        public IMachine GetMachine(Fingerprint machineFinterprint)
        {
            return _repository.GetMachine(machineFinterprint);
        }
    }
}