using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using FantasyCoachAI.Web.Filters;
using FluentAssertions;
using Moq;
using Xunit;
using System.Reflection;

namespace FantasyCoachAI.Web.Tests.Filters
{
    [Trait("Category", "Unit")]
    public class FileUploadOperationFilterTests
    {
        private readonly FileUploadOperationFilter _filter;

        public FileUploadOperationFilterTests()
        {
            _filter = new FileUploadOperationFilter();
        }

        [Fact]
        public void Apply_WhenNoFileParameters_ShouldNotModifyOperation()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>()
            };

            var context = CreateOperationFilterContext(new List<ParameterInfo>());

            // Act
            _filter.Apply(operation, context);

            // Assert
            operation.Parameters.Should().NotBeNull();
            operation.RequestBody.Should().BeNull();
        }

        [Fact]
        public void Apply_WhenSingleIFormFileParameter_ShouldCreateMultipartRequestBody()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new() { Name = "existingParam", In = ParameterLocation.Query }
                }
            };

            var fileParameter = CreateParameterInfo("file", typeof(IFormFile));
            var context = CreateOperationFilterContext(new List<ParameterInfo> { fileParameter });

            // Act
            _filter.Apply(operation, context);

            // Assert
            operation.Parameters.Should().BeEmpty(); // Parameters are cleared
            operation.RequestBody.Should().NotBeNull();
            operation.RequestBody.Content.Should().ContainKey("multipart/form-data");

            var mediaType = operation.RequestBody.Content["multipart/form-data"];
            mediaType.Schema.Should().NotBeNull();
            mediaType.Schema.Type.Should().Be("object");
            mediaType.Schema.Properties.Should().ContainKey("file");

            var fileProperty = mediaType.Schema.Properties["file"];
            fileProperty.Type.Should().Be("string");
            fileProperty.Format.Should().Be("binary");

            operation.RequestBody.Content["multipart/form-data"].Schema.Required.Should().Contain("file");
        }

        [Fact]
        public void Apply_WhenSingleIEnumerableIFormFileParameter_ShouldCreateMultipartRequestBody()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new() { Name = "existingParam", In = ParameterLocation.Query }
                }
            };

            var filesParameter = CreateParameterInfo("files", typeof(IEnumerable<IFormFile>));
            var context = CreateOperationFilterContext(new List<ParameterInfo> { filesParameter });

            // Act
            _filter.Apply(operation, context);

            // Assert
            operation.Parameters.Should().BeEmpty();
            operation.RequestBody.Should().NotBeNull();
            operation.RequestBody.Content.Should().ContainKey("multipart/form-data");

            var mediaType = operation.RequestBody.Content["multipart/form-data"];
            mediaType.Schema.Properties.Should().ContainKey("files");

            var filesProperty = mediaType.Schema.Properties["files"];
            filesProperty.Type.Should().Be("string");
            filesProperty.Format.Should().Be("binary");

            operation.RequestBody.Content["multipart/form-data"].Schema.Required.Should().Contain("files");
        }

        [Fact]
        public void Apply_WhenMultipleFileParameters_ShouldCreateRequestBodyWithAllFiles()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>()
            };

            var parameters = new List<ParameterInfo>
            {
                CreateParameterInfo("file1", typeof(IFormFile)),
                CreateParameterInfo("file2", typeof(IFormFile)),
                CreateParameterInfo("files", typeof(IEnumerable<IFormFile>))
            };

            var context = CreateOperationFilterContext(parameters);

            // Act
            _filter.Apply(operation, context);

            // Assert
            operation.RequestBody.Should().NotBeNull();
            var mediaType = operation.RequestBody.Content["multipart/form-data"];

            mediaType.Schema.Properties.Should().HaveCount(3);
            mediaType.Schema.Properties.Should().ContainKey("file1");
            mediaType.Schema.Properties.Should().ContainKey("file2");
            mediaType.Schema.Properties.Should().ContainKey("files");

            mediaType.Schema.Required.Should().HaveCount(3);
            mediaType.Schema.Required.Should().Contain(new[] { "file1", "file2", "files" });
        }

        [Fact]
        public void Apply_WhenMixedParametersIncludingNonFile_ShouldOnlyIncludeFileParameters()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>()
            };

            var parameters = new List<ParameterInfo>
            {
                CreateParameterInfo("file", typeof(IFormFile)),
                CreateParameterInfo("name", typeof(string)), // Non-file parameter
                CreateParameterInfo("count", typeof(int))     // Non-file parameter
            };

            var context = CreateOperationFilterContext(parameters);

            // Act
            _filter.Apply(operation, context);

            // Assert
            operation.RequestBody.Should().NotBeNull();
            var mediaType = operation.RequestBody.Content["multipart/form-data"];

            mediaType.Schema.Properties.Should().HaveCount(1);
            mediaType.Schema.Properties.Should().ContainKey("file");

            mediaType.Schema.Required.Should().HaveCount(1);
            mediaType.Schema.Required.Should().Contain("file");
        }

        [Fact]
        public void Apply_WhenParametersNull_ShouldNotThrowException()
        {
            // Arrange
            var operation = new OpenApiOperation();
            var context = CreateOperationFilterContext(new List<ParameterInfo>());

            // Act & Assert
            Assert.Null(operation.Parameters);
            _filter.Apply(operation, context);

            // Should not throw and should initialize Parameters
            operation.Parameters.Should().NotBeNull();
        }

        #region Helper Methods

        private OperationFilterContext CreateOperationFilterContext(List<ParameterInfo> parameters)
        {
            var methodInfoMock = new Mock<MethodInfo>();
            methodInfoMock.Setup(x => x.GetParameters()).Returns(parameters.ToArray());

            return new OperationFilterContext(
                apiDescription: null,
                schemaRegistry: null,
                schemaRepository: null,
                methodInfo: methodInfoMock.Object);
        }

        private ParameterInfo CreateParameterInfo(string name, Type parameterType)
        {
            var parameterInfoMock = new Mock<ParameterInfo>();
            parameterInfoMock.Setup(x => x.Name).Returns(name);
            parameterInfoMock.Setup(x => x.ParameterType).Returns(parameterType);

            return parameterInfoMock.Object;
        }

        #endregion
    }
}
