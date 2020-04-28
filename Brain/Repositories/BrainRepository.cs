using Brain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Repositories
{
    public interface IBrainRepository
    {
        AssociationsLookup GetCurrentAssociationsLookup(string id);
        void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup);
        SenseInputs GetLastSenseInputs(string id);
        void SaveSenseInputs(string id, SenseInputs senseInputs);
        void Initialise(string id, string[] keys);
    }

    public class BrainRepository : IBrainRepository
    {
        private Dictionary<string, AssociationsLookup> _associationsLookupById;
        private Dictionary<string, SenseInputs> _lastSenseInputsById;

        public BrainRepository()
        {
            _associationsLookupById = new Dictionary<string, AssociationsLookup>();
            _lastSenseInputsById = new Dictionary<string, SenseInputs>();
        }

        public AssociationsLookup GetCurrentAssociationsLookup(string id)
        {
            return _associationsLookupById[id];
        }

        public void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup)
        {
            _associationsLookupById[id] = associationsLookup;
        }

        public SenseInputs GetLastSenseInputs(string id)
        {
            return _lastSenseInputsById[id];
        }

        public void SaveSenseInputs(string id, SenseInputs senseInputs)
        {
            _lastSenseInputsById[id] = senseInputs;
        }

        public void Initialise(string id, string[] keys)
        {
            _lastSenseInputsById.Add(id, new SenseInputs());

            _associationsLookupById.Add(id, new AssociationsLookup());

            foreach (var key in keys)
            {
                _lastSenseInputsById[id].Add(key, 0);

                var otherKeys = keys.Where(x => x != key);
                var associationsList = otherKeys.Select(o => new KeyValuePair<string, double>(o, 1 / keys.Count()));

                _associationsLookupById[id].Add(key, new Associations(associationsList.ToDictionary(x => x.Key, x => x.Value)));
            }
        }
    }
}
