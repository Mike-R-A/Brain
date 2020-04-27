using Brain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brain.Services
{
    public interface IUpperBrainService
    {
        List<SenseInputs> GetFuturePredictedInputs(AssociationsLookup existingAssociationsLookup, SenseInputs actualInput,
            int requestedFuturePredictionLength);
        AssociationsLookup UpdateAssociationsLookup(AssociationsLookup existingAssociationsLookup, SenseInputs actualInputs,
            double weightFactor);
        int GetNoOfPredictions();
        double GetNewInputsWeightFactor();
    }

    public class UpperBrainService : IUpperBrainService
    {
        private readonly IBrainService brainService;

        public UpperBrainService(IBrainService brainService)
        {
            this.brainService = brainService;
        }

        public List<SenseInputs> GetFuturePredictedInputs(AssociationsLookup existingAssociationsLookup, SenseInputs actualInput, 
            int requestedFuturePredictionLength)
        {
            var futurePredictedInputs = new List<SenseInputs>();
            SenseInputs predictedFutureInput;
            SenseInputs nextInput = actualInput;

            for (var i = 0; i < requestedFuturePredictionLength; i++)
            {
                predictedFutureInput = brainService.GetPredictedFutureInput(nextInput, existingAssociationsLookup);
                futurePredictedInputs.Add(predictedFutureInput);
                nextInput = predictedFutureInput;
            }

            return futurePredictedInputs;
        }

        public double GetNewInputsWeightFactor()
        {
            return 0.1;
        }

        public int GetNoOfPredictions()
        {
            return 10;
        }

        public AssociationsLookup UpdateAssociationsLookup(AssociationsLookup existingAssociationsLookup, 
            SenseInputs actualInputs, double weightFactor)
        {
            var inputAssociationsLookup = brainService.CreateAssociations(actualInputs);
            var updatedAssociationsLookup = brainService.AddAndNormaliseAssociationsLookups(inputAssociationsLookup, existingAssociationsLookup, weightFactor);
            return updatedAssociationsLookup;
        }
    }
}
