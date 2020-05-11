using Brain.Model;
using Brain.Repositories;
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
        AssociationsLookup AddAssociationsLookups(AssociationsLookup lookup1, AssociationsLookup lookup2, double weightFactor);
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
                //var normalisedAssociationsDictionary = mathsService.NormaliseAssociations(new Associations(associationsDictionary));
                associations.Add(senseInput.Key, new Associations(associationsDictionary));
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
                var associationTotal = 0.0;
                foreach(var association in associationsRelatedToThisInput)
                {
                    if(association.Value > 0)
                    {
                        predictedInput += actualInput[association.Key] * association.Value;
                        associationTotal += association.Value;
                    }
                }
                predictedInput = (predictedInput + senseInput.Value) / (associationTotal + 1);

                predictedFutureInput.Add(senseInput.Key, predictedInput);
            }

            return predictedFutureInput;
        }

        public AssociationsLookup AddAssociationsLookups(AssociationsLookup lookup1, AssociationsLookup lookup2, double weightFactor)
        {
            var combinedLookups = mathsService.AddAssociationLookups(lookup1, lookup2, weightFactor);

            return combinedLookups;
        }
    }
}
