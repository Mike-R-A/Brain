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
    public class BrainServiceTests
    {
        Mock<IMathsService> mockMathsService;
        private BrainService GetService()
        {
            mockMathsService = new Mock<IMathsService>();
            return new BrainService(mockMathsService.Object);
        }

        [Fact]
        public void CreateAssociations_Should_ReturnNormalisedProducts()
        {
            var service = GetService();

            var senseInputs = new SenseInputs
            {
                { "red", 5 },
                { "green", 2 },
                { "blue", 13 }
            };

            var expectedRedNormalisedAssociations = new Associations(new Dictionary<string, double> {
                { "green", 10 },
                { "blue", 65 }
            });

            var expectedGreenNormalisedAssociations = new Associations(new Dictionary<string, double> {
                { "red", 10 },
                { "blue", 26 }
            });

            var expectedBlueNormalisedAssociations = new Associations(new Dictionary<string, double> {
                { "green", 26 },
                { "red", 65 }
            });

            var actual = service.CreateAssociations(senseInputs);

            actual["red"].Should().BeEquivalentTo(expectedRedNormalisedAssociations);
            actual["green"].Should().BeEquivalentTo(expectedGreenNormalisedAssociations);
            actual["blue"].Should().BeEquivalentTo(expectedBlueNormalisedAssociations);
        }

        [Fact]
        public void GetPredictedFutureInput_should_ReturnExpectedPrediction()
        {
            var service = GetService();

            var actualInput = new SenseInputs {
                {"yellow", 20 },
                {"volume", 50 },
                {"pain", 0 }
            };
            var associations = new AssociationsLookup {
                {
                    "yellow", 
                    new Associations
                    {
                        { "pain", 0.8 },
                        { "volume", 0.2 },
                    }
                },
                {
                    "volume",
                    new Associations
                    {
                        { "pain", 0.8 },
                        { "yellow", 0.2 },
                    }
                },
                {
                    "pain",
                    new Associations
                    {
                        { "yellow", 0.8 },
                        { "volume", 0.2 },
                    }
                }
            };

            var actual = service.GetPredictedFutureInput(actualInput, associations);

            actual["yellow"].Should().Be(15.0);
            actual["volume"].Should().Be(27.0);
            actual["pain"].Should().Be(13.0);
        }

        [Fact]
        public void AddAndNormaliseAssociationsLookups_Should_ReturnExpected()
        {
            var service = GetService();

            var associationsLookup1 = new AssociationsLookup();
            var associationsLookup2 = new AssociationsLookup();
            var weightFactor = 0.5;
            var expectedAssociationsLookup = new AssociationsLookup();

            mockMathsService.Setup(x => x.AddAssociationLookups(associationsLookup1, associationsLookup2, weightFactor))
                .Returns(expectedAssociationsLookup);

            var actual = service.AddAssociationsLookups(associationsLookup1, associationsLookup2, weightFactor);

            actual.Should().BeSameAs(expectedAssociationsLookup);
        }
    }
}
