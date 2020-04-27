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
        public AssociationsLookup GetCurrentAssociationsLookup(string id)
        {
            return _associationsLookup;
        }

        public void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup)
        {
            _associationsLookup = associationsLookup;
        }
    }
}
