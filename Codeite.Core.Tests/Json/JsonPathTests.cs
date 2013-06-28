using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeite.Core.Json;
using NUnit.Framework;
using Shouldly;

namespace Codeite.Core.Tests.Json
{
    [TestFixture]
    public class JsonPathTests
    {
        private Dictionary<string, dynamic> _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = DynamicJsonObject.ReadJson("{'a':1, 'b':2, 'c':[3, 4, 5], 'd':[{'x':6}, {'y':7}, {'z':8}]}");
        }

        [Test]
        public void CanResolveRoot()
        {
            // Arrange
            const string path = "$";

            // Act
            var result = _testObject.ReadValue(path);

            // Assert
            Assert.That(result != null);

            result.ShouldBeTypeOf<Dictionary<string, dynamic>>();
            var resultAsJsObj = (Dictionary<string, dynamic>)result;

            resultAsJsObj.ToString().ShouldBeJson("{'a':'1', 'b':2, 'c':[3, 4, 5], 'd':[{'x':6}, {'y':7}, {'z':8}]}");
        }

        [Test]
        public void CanResolveSimple()
        {
            // Arrange
            const string path = "$.a";

            // Act
            var result = _testObject.ReadValue(path);

            // Assert
            Assert.That(result != null);

            result.ShouldBeTypeOf<string>();
            var resultAsString = result as string;

            resultAsString.ShouldBe("1");
        }
    }

    public static class JsonAssertionHelper
    {
        public static void ShouldBeJson(this string actual, string expected)
        {
            var cannonicalizer = new Codeite.Core.Json.JsonCannonicalizer();
            string actualCannonical;
            string expectedCannonical;

            try
            {
                expectedCannonical = cannonicalizer.Cannonicalize(expected);
            }
            catch (DynamicJsonObjectReadException e)
            {
                throw new Exception("Expected JSON invalid, " + e.Message + "\n" + expected);
            }

            try
            {
                actualCannonical = cannonicalizer.Cannonicalize(actual);
            }
            catch (DynamicJsonObjectReadException e)
            {
                throw new Exception("Actual JSON invalid, " + e.Message + "\n" + actual);
            }

            actualCannonical.ShouldBe(expectedCannonical);
        }
    }
}
