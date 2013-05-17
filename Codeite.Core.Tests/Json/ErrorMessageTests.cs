using Codeite.Core.Json;
using NUnit.Framework;
using Shouldly;

namespace Codeite.Core.Tests.Json
{
    [TestFixture]
    public class ErrorMessageTests
    {
        [Test]
        public void ShouldReportMissingCloseBrase()
        {
            // Arrange
            const string json = @"{";
            var errorMessage = "";

            // Act
            try
            {
                DynamicJsonObject.ReadJson(json);
            }
            catch (DynamicJsonObjectReadException e)
            {
                errorMessage = e.Message;
            }

            // Assert
            errorMessage.ShouldBe("End of object or next property name expected but got: None");
        }
    }
}