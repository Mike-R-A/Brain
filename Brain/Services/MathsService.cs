using Brain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Services
{
    public interface IMathsService {
        Associations NormaliseAssociations(Associations dict);
        Associations AddAssociations(Associations dictionary1, Associations dictionary2);
        Associations ScaleAssociations(Associations dict, double factor);
        AssociationsLookup AddAssociationLookups(AssociationsLookup associationsLookup1, AssociationsLookup associationsLookup2, double weightFactor);
        SenseInputs MeanSenseInputs(SenseInputs dictionary1, SenseInputs dictionary2);
    }

    public class MathsService : IMathsService
    {
        public Associations AddAssociations(Associations dictionary1, Associations dictionary2)
        {
            var result = AddDictionaries(dictionary1, dictionary2);

            return new Associations(result);
        }

        public SenseInputs MeanSenseInputs(SenseInputs dictionary1, SenseInputs dictionary2)
        {
            var result = AddDictionaries(dictionary1, dictionary2);
            var senseInputs = new SenseInputs();

            foreach(var entry in result)
            {
                senseInputs.Add(entry.Key, entry.Value / 2);
            }

            return senseInputs;
        }

        private Dictionary<string, double> AddDictionaries(IDictionary<string, double> dictionary1, IDictionary<string, double> dictionary2)
        {
            var result = new Dictionary<string, double>();

            foreach (var keyValuePair in dictionary1)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var keyValuePair in dictionary2)
            {
                if (result.Keys.Contains(keyValuePair.Key))
                {
                    result[keyValuePair.Key] += keyValuePair.Value;
                }
                else
                {
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return result;
        }

        public Associations NormaliseAssociations(Associations dict)
        {
            var sum = dict.Values.Sum();

            if(sum == 0)
            {
                var keys = new List<string>(dict.Keys);
                foreach (var key in keys)
                {
                    dict[key] = 1 / dict.Count;
                }
                return dict;
            }

            return ScaleAssociations(dict, 1 / sum);
        }

        public Associations ScaleAssociations(Associations dict, double factor)
        {
            var list = dict.Select(k => new KeyValuePair<string, double>(k.Key, k.Value * factor));

            return new Associations(list.ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// Associations1 will be multiplied by the weightFactor then added to associations2. The result will be normalised.
        /// </summary>
        /// <param name="associations1"></param>
        /// <param name="associations2"></param>
        /// <param name="weightFactor"></param>
        /// <returns></returns>
        public AssociationsLookup AddAssociationLookups(AssociationsLookup associationsLookup1, AssociationsLookup associationsLookup2, double weightFactor)
        {
            var combinedAssociationsLookup = new AssociationsLookup();
            foreach (var associationEntry in associationsLookup1)
            {
                var key = associationEntry.Key;
                var associations = new Associations();
                var weightedAssociations1 = associationEntry.Value.Select(x => new KeyValuePair<string, double>(x.Key, x.Value * weightFactor));
                var associations2 = associationsLookup2[key];

                var combinedAssociations = weightedAssociations1.ToDictionary(x => x.Key, x => x.Value + associations2[x.Key]);

                combinedAssociationsLookup.Add(key, new Associations(combinedAssociations));
            }

            return combinedAssociationsLookup;
        }
    }
}
