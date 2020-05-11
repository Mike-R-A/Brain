using Brain.Model;
using Brain.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BrainTests.ServiceTests
{
    public class UpperBrainServiceTests
    {
        Mock<IBrainService> mockBrainService = new Mock<IBrainService>();
        private UpperBrainService GetService()
        {
            return new UpperBrainService(mockBrainService.Object);
        }

        [Fact]
        public void GetFuturePredictedInputs_Should_ReturnTheExpectedList()
        {
            var service = GetService();
            var actualInput = new SenseInputs();
            var requestedFuturePredictionLength = 3;
            var currentAssociationsLookup = new AssociationsLookup 
            {
                {
                    "key1",
                    new Associations
                    {
                        {
                            "key2", 0.1
                        },
                        {
                            "key3", 0.9
                        }
                    }
                },
                {
                    "key2",
                    new Associations
                    {
                        {
                            "key1", 0.5
                        },
                        {
                            "key3", 0.5
                        }
                    }
                },
                {
                    "key3",
                    new Associations
                    {
                        {
                            "key1", 0.3
                        },
                        {
                            "key2", 0.7
                        }
                    }
                }
            };
            var expectedFutureInput1 = new SenseInputs 
            {
                {"1", 0.1 }
            };
            var expectedFutureInput2 = new SenseInputs
            {
                {"2", 0.2 }
            };
            var expectedFutureInput3 = new SenseInputs
            {
                {"3", 0.3 }
            };

            mockBrainService.Setup(x => x.GetPredictedFutureInput(actualInput, currentAssociationsLookup))
                .Returns(expectedFutureInput1);
            mockBrainService.Setup(x => x.GetPredictedFutureInput(expectedFutureInput1, currentAssociationsLookup))
                .Returns(expectedFutureInput2);
            mockBrainService.Setup(x => x.GetPredictedFutureInput(expectedFutureInput2, currentAssociationsLookup))
                .Returns(expectedFutureInput3);

            List<SenseInputs> actualPredictedFuture = service.GetFuturePredictedInputs(currentAssociationsLookup, actualInput, requestedFuturePredictionLength);

            actualPredictedFuture[0].Should().BeSameAs(expectedFutureInput1);
            actualPredictedFuture[1].Should().BeSameAs(expectedFutureInput2);
            actualPredictedFuture[2].Should().BeSameAs(expectedFutureInput3);
        }

        [Fact]
        public void UpdateAssociationsLookup_Should_AddAsExpectedAndSave()
        {
            var service = GetService();
            var actualInputs = new SenseInputs();
            var existingAssociationsLookup = new AssociationsLookup();
            var inputAssociationsLookup = new AssociationsLookup();
            var combinedNormalisedLookup = new AssociationsLookup();
            var weightFactor = 0.2;

            mockBrainService.Setup(x => x.CreateAssociations(actualInputs)).Returns(inputAssociationsLookup);
            mockBrainService.Setup(x => x.AddAssociationsLookups(inputAssociationsLookup, 
                existingAssociationsLookup, weightFactor)).Returns(combinedNormalisedLookup);

            var actual = service.UpdateAssociationsLookup(existingAssociationsLookup, actualInputs, weightFactor);

            actual.Should().BeSameAs(combinedNormalisedLookup);
        }
    }
}
