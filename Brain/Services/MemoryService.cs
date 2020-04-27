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
    }

    public class MemoryService : IMemoryService
    {
        private readonly IUpperBrainService upperBrainService;
        private readonly IBrainRepository brainRepository;

        public MemoryService(IUpperBrainService upperBrainService, IBrainRepository brainRepository)
        {
            this.upperBrainService = upperBrainService;
            this.brainRepository = brainRepository;
        }

        public List<SenseInputs> ManageSenseInputs(string id, SenseInputs senseInputs)
        {
            var requestedFuturePredictions = upperBrainService.GetNoOfPredictions();
            var newInputsWeightFactor = upperBrainService.GetNewInputsWeightFactor();
            var existingAssociationsLookup = brainRepository.GetCurrentAssociationsLookup(id);
            var futurePredictedInputs = upperBrainService.GetFuturePredictedInputs(existingAssociationsLookup, senseInputs, requestedFuturePredictions);
            var updatedAssociationsLookup = upperBrainService.UpdateAssociationsLookup(existingAssociationsLookup, senseInputs, newInputsWeightFactor);
            brainRepository.SaveAssociationsLookup(id, updatedAssociationsLookup);

            return futurePredictedInputs;
        }
    }
}
