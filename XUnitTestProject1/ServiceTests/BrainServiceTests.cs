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
                { "expectedRed", 15 }
            });

            var expectedGreenNormalisedAssociations = new Associations(new Dictionary<string, double> {
                { "expectedGreen", 199 }
            });

            var expectedBlueNormalisedAssociations = new Associations(new Dictionary<string, double> {
                { "expectedBlue", 77 }
            });

            mockMathsService.Setup(x => x.NormaliseAssociations(It.Is<Associations>(
                d => d.ContainsKey("green") ? d["green"] == 10 : false
                && d.ContainsKey("blue") ? d["blue"] == 65 : false))).Returns(expectedRedNormalisedAssociations);
            mockMathsService.Setup(x => x.NormaliseAssociations(It.Is<Associations>(
                d => d.ContainsKey("red") ? d["red"] == 10 : false
                && d.ContainsKey("blue") ? d["blue"] == 26 : false))).Returns(expectedGreenNormalisedAssociations);
            mockMathsService.Setup(x => x.NormaliseAssociations(It.Is<Associations>(
                d => d.ContainsKey("green") ? d["green"] == 26 : false
                && d.ContainsKey("red") ? d["red"] == 65 : false))).Returns(expectedBlueNormalisedAssociations);

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

            actual["yellow"].Should().Be(5);
            actual["volume"].Should().Be(2);
            actual["pain"].Should().Be(13);
        }
    }
}
