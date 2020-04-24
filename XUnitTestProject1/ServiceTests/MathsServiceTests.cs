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

        [Theory]
        [InlineData("1,1,1,1", "0.25,0.25,0.25,0.25")]
        [InlineData("0.5,0.5", "0.5,0.5")]
        public void NormaliseArray_Should_ReturnNormalisedIEnumerable(string inputAsString, string expectedAsString)
        {
            var service = GetService();

            var array = inputAsString.Split(',').Select(x => Convert.ToDouble(x));

            var actual = service.NormaliseArray(array);
            var expected = expectedAsString.Split(',').Select(x => Convert.ToDouble(x));

            actual.Count().Should().Be(array.Count());
            actual.Sum().Should().Be(1);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void AddDictionaries_Should_ReturnDictionaryWithValuesAddedByKey()
        {
            var service = GetService();

            var associationsMemory = new Dictionary<string, double>
            {
                { "redLight", 0.1 },
                { "greenLight", 0.8 },
                { "blueLight", 0.1 }
            };
            var associationsNew = new Dictionary<string, double>
            {
                { "redLight", 0.5 },
                { "greenLight", 0.2 },
                { "blueLight", 0.3 }
            };

            var actual = service.AddDictionaries(associationsNew, associationsMemory);

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

            var dict = new Dictionary<string, double>
            {
                { "key1", 1 },
                { "key2", 2 },
                { "key3", 2 }
            };

            var actual = service.NormaliseDictionary(dict);

            actual.Count().Should().Be(dict.Count);
            actual["key1"].Should().Be(0.2);
            actual["key2"].Should().Be(0.4);
            actual["key3"].Should().Be(0.4);
        }

        [Fact]
        public void ScaleDictionary_Should_ReturnAScaledDictionary()
        {
            var service = GetService();

            var dict = new Dictionary<string, double>
            {
                { "key1", 1 },
                { "key2", 7 },
                { "key3", 2 }
            };

            var factor = 4;

            var actual = service.ScaleDictionary(dict, factor);

            actual["key1"].Should().Be(4);
            actual["key2"].Should().Be(28);
            actual["key3"].Should().Be(8);
        }
    }
}
