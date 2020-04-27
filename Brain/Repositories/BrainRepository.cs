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
    }

    public class BrainRepository : IBrainRepository
    {
        private AssociationsLookup _associationsLookup;

        public BrainRepository()
        {
            var keys = new string[] 
            {
                "red",
                "green",
                "pleasure",
                "pain"
            };

            InitialiseAssociations(keys);
        }

        public AssociationsLookup GetCurrentAssociationsLookup(string id)
        {
            return _associationsLookup;
        }

        public void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup)
        {
            _associationsLookup = associationsLookup;
        }

        public void InitialiseAssociations(string[] keys)
        {
            _associationsLookup = new AssociationsLookup();

            foreach (var key in keys)
            {
                var otherKeys = keys.Where(x => x != key);
                var associationsList = otherKeys.Select(o => new KeyValuePair<string, double>(o, 1 / keys.Count()));

                _associationsLookup.Add(key, new Associations(associationsList.ToDictionary(x => x.Key, x => x.Value)));
            }
        }
    }
}
