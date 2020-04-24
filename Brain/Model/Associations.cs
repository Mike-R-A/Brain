using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Model
{
    public class Associations: Dictionary<string, double>
    {
        public Associations(IDictionary<string, double> dict = null)
        {
            if(dict != null)
            {
                foreach (var entry in dict)
                {
                    Add(entry.Key, entry.Value);
                }
            }
        }
    }
}
