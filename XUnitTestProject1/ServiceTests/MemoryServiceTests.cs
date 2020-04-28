using Brain.Model;
using Brain.Repositories;
using Brain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BrainTests.ServiceTests
{
    public class MemoryServiceTests
    {
        Mock<IUpperBrainService> mockUpperBrainService = new Mock<IUpperBrainService>();
        Mock<IBrainRepository> mockBrainRepository = new Mock<IBrainRepository>();
        Mock<IMathsService> mockMathsService = new Mock<IMathsService>();
        private MemoryService GetService()
        {
            return new MemoryService(mockUpperBrainService.Object, mockBrainRepository.Object, mockMathsService.Object);
        }

        [Fact]
        public void ManageSenseInputs_Should_ReturnPredictedAndUpdateExisting()
        {
            var service = GetService();
            var senseInputs = new SenseInputs();
            var id = "972f97";
            var requestedFuturePredictions = 2;
            var newInputsWeightFactor = 0.1;
            var expectedExistingAssociationsLookup = new AssociationsLookup();
            var expectedUpdatedAssociationsLookup = new AssociationsLookup();
            var expectedFuturePredictedInputs = new List<SenseInputs>();
            var expectedLastInputs = new SenseInputs();
            var expectedCombinedInputs = new SenseInputs();

            mockBrainRepository.Setup(x => x.GetLastSenseInputs(id)).Returns(expectedLastInputs);
            mockMathsService.Setup(x => x.MeanSenseInputs(senseInputs, expectedLastInputs)).Returns(expectedCombinedInputs);
            mockBrainRepository.Setup(x => x.GetCurrentAssociationsLookup(id)).Returns(expectedExistingAssociationsLookup);
            mockUpperBrainService.Setup(x => x.GetFuturePredictedInputs(expectedExistingAssociationsLookup,
                expectedCombinedInputs, requestedFuturePredictions)).Returns(expectedFuturePredictedInputs);
            mockUpperBrainService.Setup(x => x.UpdateAssociationsLookup(expectedExistingAssociationsLookup, expectedCombinedInputs, 
                newInputsWeightFactor)).Returns(expectedUpdatedAssociationsLookup);
            mockUpperBrainService.Setup(x => x.GetNoOfPredictions()).Returns(requestedFuturePredictions);
            mockUpperBrainService.Setup(x => x.GetNewInputsWeightFactor()).Returns(newInputsWeightFactor);

            List<SenseInputs> actual = service.ManageSenseInputs(id, senseInputs);

            mockBrainRepository.Verify(x => x.SaveAssociationsLookup(id, expectedUpdatedAssociationsLookup), Times.Once);
            actual.Should().BeSameAs(expectedFuturePredictedInputs);
        }
    }
}
