using Brain.Model;
using Brain.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BrainTests.ServiceTests
{
    public class MathsServiceTests
    {
        private MathsService GetService()
        {
            return new MathsService();
        }

        [Fact]
        public void AddDictionaries_Should_ReturnDictionaryWithValuesAddedByKey()
        {
            var service = GetService();

            var associationsMemory = new Associations
            {
                { "redLight", 0.1 },
                { "greenLight", 0.8 },
                { "blueLight", 0.1 }
            };
            var associationsNew = new Associations
            {
                { "redLight", 0.5 },
                { "greenLight", 0.2 },
                { "blueLight", 0.3 }
            };

            var actual = service.AddAssociations(associationsNew, associationsMemory);

            foreach(var keyValuePair in actual)
            {
                var expectedValue = associationsMemory[keyValuePair.Key] + associationsNew[keyValuePair.Key];
                keyValuePair.Value.Should().Be(expectedValue);
            }
        }

        [Fact]
        public void NormaliseDictionary_Should_NormaliseADictionary()
        {
            var service = GetService();

            var dict = new Associations
            {
                { "key1", 1 },
                { "key2", 2 },
                { "key3", 2 }
            };

            var actual = service.NormaliseAssociations(dict);

            actual.Count().Should().Be(dict.Count);
            actual["key1"].Should().Be(0.2);
            actual["key2"].Should().Be(0.4);
            actual["key3"].Should().Be(0.4);
        }

        [Fact]
        public void ScaleDictionary_Should_ReturnAScaledDictionary()
        {
            var service = GetService();

            var dict = new Associations
            {
                { "key1", 1 },
                { "key2", 7 },
                { "key3", 2 }
            };

            var factor = 4;

            var actual = service.ScaleAssociations(dict, factor);

            actual["key1"].Should().Be(4);
            actual["key2"].Should().Be(28);
            actual["key3"].Should().Be(8);
        }

        [Fact]
        public void AddAssociations_Should_AddUsingWeightAndNormalise()
        {
            var service = GetService();

            var associations1 = new AssociationsLookup
            {
                {
                    "red",
                    new Associations
                    {
                        {
                            "blue", 0.1
                        },
                        {
                            "green", 0.9
                        }
                    }
                },
                {
                    "blue",
                    new Associations
                    {
                        {
                            "red", 0.1
                        },
                        {
                            "green", 0.9
                        }
                    }
                },
                {
                    "green",
                    new Associations
                    {
                        {
                            "red", 0.9
                        },
                        {
                            "blue", 0.1
                        }
                    }
                }
            };

            var associations2 = new AssociationsLookup
            {
                {
                    "red",
                    new Associations
                    {
                        {
                            "blue", 0.5
                        },
                        {
                            "green", 0.5
                        }
                    }
                },
                {
                    "blue",
                    new Associations
                    {
                        {
                            "red", 0.5
                        },
                        {
                            "green", 0.5
                        }
                    }
                },
                {
                    "green",
                    new Associations
                    {
                        {
                            "red", 0.5
                        },
                        {
                            "blue", 0.5
                        }
                    }
                }
            };

            var weightFactor = 0.1;

            AssociationsLookup actual = service.AddAssociationLookups(associations1, associations2, weightFactor);

            actual["red"]["blue"].Should().Be(0.51);
            actual["red"]["green"].Should().Be(0.59);
            actual["blue"]["red"].Should().Be(0.51);
            actual["blue"]["green"].Should().Be(0.59);
            actual["green"]["red"].Should().Be(0.59);
            actual["green"]["blue"].Should().Be(0.51);
        }
    }
}
