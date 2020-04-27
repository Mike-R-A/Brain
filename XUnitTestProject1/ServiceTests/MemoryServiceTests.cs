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
        private MemoryService GetService()
        {
            return new MemoryService(mockUpperBrainService.Object, mockBrainRepository.Object);
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

            mockBrainRepository.Setup(x => x.GetCurrentAssociationsLookup(id)).Returns(expectedExistingAssociationsLookup);
            mockUpperBrainService.Setup(x => x.GetFuturePredictedInputs(expectedExistingAssociationsLookup,
                senseInputs, requestedFuturePredictions)).Returns(expectedFuturePredictedInputs);
            mockUpperBrainService.Setup(x => x.UpdateAssociationsLookup(expectedExistingAssociationsLookup, senseInputs, 
                newInputsWeightFactor)).Returns(expectedUpdatedAssociationsLookup);

            List<SenseInputs> actual = service.ManageSenseInputs(id, senseInputs, requestedFuturePredictions, newInputsWeightFactor);

            mockBrainRepository.Verify(x => x.SaveAssociationsLookup(id, expectedUpdatedAssociationsLookup), Times.Once);
            actual.Should().BeSameAs(expectedFuturePredictedInputs);
        }
    }
}
