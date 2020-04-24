using Brain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Services
{
    public interface IBrainService
    {
        AssociationsLookup CreateAssociations(SenseInputs senseInputs);
        SenseInputs GetPredictedFutureInput(SenseInputs actualInput, 
            AssociationsLookup associations);

    }
    public class BrainService : IBrainService
    {
        private readonly IMathsService mathsService;

        public BrainService(IMathsService mathsService)
        {
            this.mathsService = mathsService;
        }
        public AssociationsLookup CreateAssociations(SenseInputs senseInputs)
        {
            var associations = new AssociationsLookup();

            foreach(var senseInput in senseInputs)
            {
                var otherSenseInputs = senseInputs.Where(s => s.Key != senseInput.Key);
                var associationsDictionary = otherSenseInputs
                    .Select(o => new KeyValuePair<string, double>(o.Key, o.Value * senseInput.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
                var normalisedAssociationsDictionary = mathsService.NormaliseDictionary(associationsDictionary);
                associations.Add(senseInput.Key, new Associations(normalisedAssociationsDictionary));
            }

            return associations;
        }

        public SenseInputs GetPredictedFutureInput(SenseInputs actualInput, 
            AssociationsLookup associations)
        {
            var predictedFutureInput = new SenseInputs();
            foreach(var senseInput in actualInput)
            {
                double predictedInput = 0;
                var associationsRelatedToThisInput = associations[senseInput.Key];
                foreach(var association in associationsRelatedToThisInput)
                {
                    predictedInput += actualInput[association.Key] * association.Value;
                }
                predictedInput = predictedInput / associationsRelatedToThisInput.Count;

                predictedFutureInput.Add(senseInput.Key, predictedInput);
            }

            return predictedFutureInput;
        }
    }
}
