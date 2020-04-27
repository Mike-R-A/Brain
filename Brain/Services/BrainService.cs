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
        AssociationsLookup AddAndNormaliseAssociationsLookups(AssociationsLookup lookup1, AssociationsLookup lookup2, double weightFactor);
        AssociationsLookup GetCurrentAssociationsLookup(string id);
        void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup);
    }
    public class BrainService : IBrainService
    {
        private readonly IMathsService mathsService;
        private readonly IBrainRepository brainRepository;

        public BrainService(IMathsService mathsService, IBrainRepository brainRepository)
        {
            this.mathsService = mathsService;
            this.brainRepository = brainRepository;
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
                var normalisedAssociationsDictionary = mathsService.NormaliseAssociations(new Associations(associationsDictionary));
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
                predictedInput += senseInput.Value;
                predictedInput = predictedInput / actualInput.Count;

                predictedFutureInput.Add(senseInput.Key, predictedInput);
            }

            return predictedFutureInput;
        }

        public AssociationsLookup AddAndNormaliseAssociationsLookups(AssociationsLookup lookup1, AssociationsLookup lookup2, double weightFactor)
        {
            var normalisedCombinedLookups = new AssociationsLookup();
            var combinedLookups = mathsService.AddAssociationLookups(lookup1, lookup2, weightFactor);
            foreach(var lookup in combinedLookups)
            {
                normalisedCombinedLookups.Add(lookup.Key, mathsService.NormaliseAssociations(lookup.Value));
            }

            return normalisedCombinedLookups;
        }

        public AssociationsLookup GetCurrentAssociationsLookup(string id)
        {
            return brainRepository.GetCurrentAssociationsLookup(id);
        }

        public void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup)
        {
            brainRepository.SaveAssociationsLookup(id, associationsLookup);
        }
    }
}
