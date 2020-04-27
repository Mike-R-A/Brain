using Brain.Model;
using Brain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Services
{
    public interface IMemoryService
    {
        List<SenseInputs> ManageSenseInputs(string id, SenseInputs senseInputs);
        AssociationsLookup GetCurrentAssociationsLookup(string id);
        void SaveAssociationsLookup(string id, AssociationsLookup associationsLookup);
    }

    public class MemoryService
    {
        private readonly IUpperBrainService upperBrainService;
        private readonly IBrainRepository brainRepository;

        public MemoryService(IUpperBrainService upperBrainService, IBrainRepository brainRepository)
        {
            this.upperBrainService = upperBrainService;
            this.brainRepository = brainRepository;
        }

        public List<SenseInputs> ManageSenseInputs(string id, SenseInputs senseInputs, int requestedFuturePredictions, double newInputsWeightFactor)
        {
            var existingAssociationsLookup = brainRepository.GetCurrentAssociationsLookup(id);
            var futurePredictedInputs = upperBrainService.GetFuturePredictedInputs(existingAssociationsLookup, senseInputs, requestedFuturePredictions);
            var updatedAssociationsLookup = upperBrainService.UpdateAssociationsLookup(existingAssociationsLookup, senseInputs, newInputsWeightFactor);
            brainRepository.SaveAssociationsLookup(id, updatedAssociationsLookup);

            return futurePredictedInputs;
        }
    }
}
