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
        Mock<IBrainRepository> mockBrainRepository;
        private BrainService GetService()
        {
            mockMathsService = new Mock<IMathsService>();
            mockBrainRepository = new Mock<IBrainRepository>();
            return new BrainService(mockMathsService.Object, mockBrainRepository.Object);
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

            actual["yellow"].Should().Be(10);
            actual["volume"].Should().Be(18);
            actual["pain"].Should().Be(26.0 / 3.0);
        }

        [Fact]
        public void AddAndNormaliseAssociationsLookups_Should_ReturnExpected()
        {
            var service = GetService();

            var associationsLookup1 = new AssociationsLookup();
            var associationsLookup2 = new AssociationsLookup();
            var weightFactor = 0.5;
            var expectedAssociations1 = new Associations
                    {
                        {
                            "no2", 0.1
                        }
                    };
            var expectedAssociations2 = new Associations
                    {
                        {
                            "no1", 0.9
                        }
                    };
            var expectedAssociationsLookup = new AssociationsLookup 
            {
                { 
                    "no1", 
                    expectedAssociations1
                },
                {
                    "no2",
                    expectedAssociations2
                }
            };
            var expectedNormalised1 = new Associations();
            var expectedNormalised2 = new Associations();

            mockMathsService.Setup(x => x.AddAssociationLookups(associationsLookup1, associationsLookup2, weightFactor))
                .Returns(expectedAssociationsLookup);
            mockMathsService.Setup(x => x.NormaliseAssociations(expectedAssociations1)).Returns(expectedNormalised1);
            mockMathsService.Setup(x => x.NormaliseAssociations(expectedAssociations2)).Returns(expectedNormalised2);

            var actual = service.AddAndNormaliseAssociationsLookups(associationsLookup1, associationsLookup2, weightFactor);

            actual["no1"].Should().BeSameAs(expectedNormalised1);
            actual["no2"].Should().BeSameAs(expectedNormalised2);
            actual.Count.Should().Be(2);
        }

        [Fact]
        public void GetCurrentAssociationsLookup_Should_ReturnFromTheRepository()
        {
            var service = GetService();
            var expected = new AssociationsLookup();
            var id = "12414fwf";

            mockBrainRepository.Setup(x => x.GetCurrentAssociationsLookup(id)).Returns(expected);

            var actual = service.GetCurrentAssociationsLookup(id);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void SaveAssociationsLookup_Should_SaveToTheRepository()
        {
            var service = GetService();
            var expectedLookup = new AssociationsLookup();
            var expectedId = "12414fwf";

            service.SaveAssociationsLookup(expectedId, expectedLookup);

            mockBrainRepository.Verify(x => x.SaveAssociationsLookup(expectedId, expectedLookup));
        }
    }
}
