using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeite.Core.Json;
using NUnit.Framework;
using Shouldly;

namespace Codeite.Core.Tests.Json
{
    [TestFixture]
    public class DynamicJsonObjectTests
    {
        [Test]
        public void CanReadJsonString()
        {
            // Arrange
            const string json = @"""my first string""";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            var stringValue = value as string;
            ShouldBeTestExtensions.ShouldBeTypeOf<string>(value);
            stringValue.ShouldBe("my first string");
        }

        [Test]
        public void CanReadJsonNumber()
        {
            // Arrange
            const string json = @"12345";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<long>(value);
            var intValue = (long)value;
            intValue.ShouldBe(12345);
        }

        [Test]
        public void CanReadJsonDouble()
        {
            // Arrange
            const string json = @"12345.55";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<double>(value);
            var floatValue = (double)value;
            floatValue.ShouldBe(12345.55);
        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        public void CanReadJsonBool(string json, bool result)
        {
            // Arrange

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<bool>(value);
            var boolValue = (bool)value;
            boolValue.ShouldBe(result);
        }

        [Test]
        public void CanReadJsonNull()
        {
            // Arrange
            const string json = @"null";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            var objectValue = value as object;
            objectValue.ShouldBe(null);
        }

        [Test]
        public void CanReadEmptyObject()
        {
            // Arrange
            const string json = @"{}";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<Dictionary<string, dynamic>>(value);
            var jsonObject = value as Dictionary<string, dynamic>;
            jsonObject.ShouldNotBe(null);
        }

        [Test]
        public void CanReadObjectWithOneProperty()
        {
            // Arrange
            const string json = @"{""alpha"":1}";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<Dictionary<string, dynamic>>(value);
            var jsonObject = (Dictionary<string, dynamic>)value;
            jsonObject.ShouldNotBe(null);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject["alpha"]);
            long firstPropValue = jsonObject["alpha"];
            firstPropValue.ShouldBe(1);
        }

        [Test]
        public void CanReadObjectWithTwoProperties()
        {
            // Arrange
            const string json = @"{""alpha"":1, ""beta"": 2}";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<Dictionary<string, dynamic>>(value);
            var jsonObject = (Dictionary<string, dynamic>)value;
            jsonObject.ShouldNotBe(null);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject["alpha"]);
            long firstPropValue = jsonObject["alpha"];
            firstPropValue.ShouldBe(1);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject["beta"]);
            long secondPropValue = jsonObject["beta"];
            secondPropValue.ShouldBe(2);
        }

        [Test]
        public void CanReadEmptyArray()
        {
            // Arrange
            const string json = @"[]";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<List<dynamic>>(value);
            var jsonObject = value as List<dynamic>;
            jsonObject.ShouldNotBe(null);
        }

        [Test]
        public void CanReadArrayWithOneProperty()
        {
            // Arrange
            const string json = @"[1]";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<List<dynamic>>(value);
            var jsonObject = (List<dynamic>)value;
            jsonObject.ShouldNotBe(null);
            jsonObject.Count.ShouldBe(1);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject[0]);
            long firstPropValue = jsonObject[0];
            firstPropValue.ShouldBe(1);
        }

        [Test]
        public void CanReadArrayWithTwoProperties()
        {
            // Arrange
            const string json = @"[1, 2]";

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<List<dynamic>>(value);
            var jsonObject = (List<dynamic>)value;
            jsonObject.ShouldNotBe(null);
            jsonObject.Count.ShouldBe(2);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject[0]);
            long firstPropValue = jsonObject[0];
            firstPropValue.ShouldBe(1);

            ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject[1]);
            long secondPropValue = jsonObject[1];
            secondPropValue.ShouldBe(2);
        }

        [Test]
        public void CanReadArrayWithManyProperties()
        {
            // Arrange
            var json = "[" + string.Join(", ", Enumerable.Range(0, 50).Select(i => i * 2)) + "]";
            Console.WriteLine(json);

            // Act
            dynamic value = DynamicJsonObject.ReadJson(json);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<List<dynamic>>(value);
            var jsonObject = (List<dynamic>)value;
            jsonObject.ShouldNotBe(null);
            jsonObject.Count.ShouldBe(50);

            for (int i = 0; i < 50; i++)
            {
                ShouldBeTestExtensions.ShouldBeTypeOf<long>(jsonObject[i]);
                long arrayValue = jsonObject[i];
                arrayValue.ShouldBe(i * 2);
            }
        }

        [Test]
        public void CanReadSampleJson()
        {
            // Arrange
            var sampleJson = File.ReadAllText(@"json\sample.json");

            // Act
            dynamic value = DynamicJsonObject.ReadJson(sampleJson);

            // Assert
            ShouldBeTestExtensions.ShouldBeTypeOf<Dictionary<string, dynamic>>(value);
            var jsonObject = (Dictionary<string, dynamic>)value;
            jsonObject.ShouldNotBe(null);

            Console.WriteLine(string.Join("\n", jsonObject.Keys));
        }
    }
}
