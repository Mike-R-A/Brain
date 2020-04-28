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
        private readonly IMathsService mathsService;

        public MemoryService(IUpperBrainService upperBrainService, IBrainRepository brainRepository, IMathsService mathsService)
        {
            this.upperBrainService = upperBrainService;
            this.brainRepository = brainRepository;
            this.mathsService = mathsService;
        }

        public List<SenseInputs> ManageSenseInputs(string id, SenseInputs senseInputs)
        {
            var lastInputs = brainRepository.GetLastSenseInputs(id);
            foreach(var key in lastInputs.Keys)
            {
                if (!senseInputs.ContainsKey(key))
                {
                    senseInputs.Add(key, 0);
                }
            }
            var combinedInputs = mathsService.MeanSenseInputs(senseInputs, lastInputs);
            brainRepository.SaveSenseInputs(id, combinedInputs);
            var requestedFuturePredictions = upperBrainService.GetNoOfPredictions();
            var newInputsWeightFactor = upperBrainService.GetNewInputsWeightFactor();
            var existingAssociationsLookup = brainRepository.GetCurrentAssociationsLookup(id);
            var futurePredictedInputs = upperBrainService.GetFuturePredictedInputs(existingAssociationsLookup, combinedInputs, requestedFuturePredictions);
            var updatedAssociationsLookup = upperBrainService.UpdateAssociationsLookup(existingAssociationsLookup, combinedInputs, newInputsWeightFactor);
            brainRepository.SaveAssociationsLookup(id, updatedAssociationsLookup);

            return futurePredictedInputs;
        }
    }
}
