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
            _associationsLookup = new AssociationsLookup
            {
                {
                    "red",
                    new Associations
                    {
                        {
                            "pain", 0.3333333333
                        },
                        {
                            "green", 0.3333333333
                        },
                        {
                            "pleasure", 0.3333333333
                        }
                    }
                },
                {
                    "pain",
                    new Associations
                    {
                        {
                            "red", 0.3333333333
                        },
                        {
                            "green", 0.3333333333
                        },
                        {
                            "pleasure", 0.3333333333
                        }
                    }
                },
                {
                    "green",
                    new Associations
                    {
                        {
                            "pain", 0.3333333333
                        },
                        {
                            "red", 0.3333333333
                        },
                        {
                            "pleasure", 0.3333333333
                        }
                    }
                },
                {
                    "pleasure",
                    new Associations
                    {
                        {
                            "pain", 0.3333333333
                        },
                        {
                            "green", 0.3333333333
                        },
                        {
                            "red", 0.3333333333
                        }
                    }
                }
            };
        }

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
