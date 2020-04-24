using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Services
{
    public interface IMathsService {
        IEnumerable<double> NormaliseArray(IEnumerable<double> inputArray);
        IDictionary<string, double> NormaliseDictionary(IDictionary<string, double> dict);
        IDictionary<string, double> AddDictionaries(IDictionary<string, double> dictionary1, IDictionary<string, double> dictionary2);
        IDictionary<string, double> ScaleDictionary(IDictionary<string, double> dict, double factor);
    }

    public class MathsService : IMathsService
    {
        public IEnumerable<double> NormaliseArray(IEnumerable<double> inputArray)
        {
            var sum = inputArray.Sum();
            return inputArray.Select(x => x / sum);
        }

        public IDictionary<string, double> AddDictionaries(IDictionary<string, double> dictionary1, IDictionary<string, double> dictionary2)
        {
            var result = new Dictionary<string, double>();

            foreach(var keyValuePair in dictionary1)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var keyValuePair in dictionary2)
            {
                if (result.Keys.Contains(keyValuePair.Key))
                {
                    result[keyValuePair.Key] += keyValuePair.Value;
                } else
                {
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return result;
        }

        public IDictionary<string, double> NormaliseDictionary(IDictionary<string, double> dict)
        {
            var sum = dict.Values.Sum();

            return ScaleDictionary(dict, 1 / sum);
        }

        public IDictionary<string, double> ScaleDictionary(IDictionary<string, double> dict, double factor)
        {
            var list = dict.Select(k => new KeyValuePair<string, double>(k.Key, k.Value * factor));

            return list.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
