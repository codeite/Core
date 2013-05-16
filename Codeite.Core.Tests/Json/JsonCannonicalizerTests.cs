using System;
using System.IO;
using Codeite.Core.Json;
using NUnit.Framework;
using Shouldly;

namespace Codeite.Core.Tests.Json
{
    public class JsonCannonicalizerTests
    {
        [Test]
        public void ShouldFixWhiteSpaceInObject()
        {
            // Arrange
            const string rawJson = @"{ ""a"" :   ""1"", ""b"" :""2"",""c"":""3""}";
            var cannonicalizer = new JsonCannonicalizer();

            // Act
            var cannonical = cannonicalizer.Cannonicalize(rawJson);

            // Assert
            cannonical.ShouldBe(@"{""a"":""1"",""b"":""2"",""c"":""3""}");
        }

        [Test]
        public void ShouldFixWhiteSpaceInArray()
        {
            // Arrange
            const string rawJson = @"[ 1, 2, 3 , 4  ,   5      ]";
            var cannonicalizer = new JsonCannonicalizer();

            // Act
            var cannonical = cannonicalizer.Cannonicalize(rawJson);

            // Assert
            cannonical.ShouldBe(@"[1,2,3,4,5]");
        }

        [TestCase(" 1 ", "1")]
        [TestCase(" 1000000000 ", "1000000000")]
        [TestCase(" 1.5 ", "1.5")]
        [TestCase(" true ", "true")]
        [TestCase(" false ", "false")]
        [TestCase(" null ", "null")]
        [TestCase(" {   } ", "{}")]
        [TestCase(" [   ] ", "[]")]
        [TestCase(" \" bob jones \" ", "\" bob jones \"")]
        [TestCase(" { a : 55  } ", "{\"a\":55}")]
        public void CanDoSimpleCase(string raw, string expectedCannonical)
        {
            // Arrange
            var cannonicalizer = new JsonCannonicalizer();

            // Act
            var cannonical = cannonicalizer.Cannonicalize(raw);

            // Assert
            cannonical.ShouldBe(expectedCannonical);
        }

        [TestCase(" 1e5 ", "100000")]
        [TestCase(" 111111111111111111111111111111111111111111111111111 ", "111111111111111111111111111111111111111111111111111")]
        [TestCase(@"""\u0064""", "\"d\"")]
        [TestCase(@"""\u0064\u0000\u9999""", "\"d\0\u9999\"")]
        [TestCase("'x'", "\"x\"")]
        public void CanDoEdgeCase(string raw, string expectedCannonical)
        {
            // Arrange
            var cannonicalizer = new JsonCannonicalizer();

            // Act
            var cannonical = cannonicalizer.Cannonicalize(raw);

            // Assert
            cannonical.ShouldBe(expectedCannonical);
        }

        [Test]
        public void SortsObjectProperties()
        {
            // Arrange
            const string rawJson = @"{ ""c"" :   ""1"", ""b"" :""2"",""a"":""3""}";
            var cannonicalizer = new JsonCannonicalizer();

            // Act
            string cannonical = cannonicalizer.Cannonicalize(rawJson);
            string toString = DynamicJsonObject.ToJsonString(DynamicJsonObject.ReadJson(rawJson));

            // Assert
            cannonical.ShouldBe(@"{""a"":""3"",""b"":""2"",""c"":""1""}");
            toString.ShouldBe(@"{""c"":""1"",""b"":""2"",""a"":""3""}");
        }

        [Test]
        public void SampleJsonDoesNotCrash()
        {
            // Arrange
            var sampleJson = File.ReadAllText(@"json\sample.json");

            // Act
            var cannonicalizer = new JsonCannonicalizer();
            var cannonical = cannonicalizer.Cannonicalize(sampleJson);

            // Assert
            cannonical.ShouldNotBe(null);
        }
    }
}
