using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yafa.Application.Common.Utility;

namespace Yafa.Tests.Services
{
    public class ApiResponseServiceTests
    {
        private readonly ApiResponseService _sut;

        public ApiResponseServiceTests()
        {
            _sut = new ApiResponseService();
        }

        [Fact]
        public void SuccessResponse_WhenCalledWithObject_ShouldReturnOkStatusCode()
        {
            // Arrange
            var data = new { Name = "Test" };

            // Act
            var actionResult = _sut.SuccessResponse(data);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;  // ← .Result not .Value
            var response = okResult!.Value as APIResponse;

            response!.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void SuccessResponse_WhenCalledWithObject_ShouldSetIsSuccessTrue()
        {
            // Arrange
            var data = new { Name = "Test" };

            // Act
            var actionResult = _sut.SuccessResponse(data);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;

            response!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void SuccessResponse_WhenCalledWithString_ShouldReturnStringInResult()
        {
            // Arrange
            var message = "Registration Succesfully";

            // Act
            var actionResult = _sut.SuccessResponse(message);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;

            response!.Result.Should().Be("Registration Succesfully");
        }

        // ── BadRequestResponse ───────────────────────────────────────

        [Fact]
        public void BadRequestResponse_WhenCalled_ShouldReturnBadRequestStatusCode()
        {
            // Arrange
            var errorMessage = "UserId is required.";

            // Act
            var actionResult = _sut.BadRequestResponse(errorMessage);

            // Assert
            var badResult = actionResult.Result as BadRequestObjectResult;  // ← BadRequestObjectResult
            var response = badResult!.Value as APIResponse;

            response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void BadRequestResponse_WhenCalled_ShouldSetIsSuccessFalse()
        {
            // Act
            var actionResult = _sut.BadRequestResponse("some error");

            // Assert
            var badResult = actionResult.Result as BadRequestObjectResult;
            var response = badResult!.Value as APIResponse;

            response!.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void BadRequestResponse_WhenCalled_ShouldContainErrorMessage()
        {
            // Arrange
            var errorMessage = "UserId is required.";

            // Act
            var actionResult = _sut.BadRequestResponse(errorMessage);

            // Assert
            var badResult = actionResult.Result as BadRequestObjectResult;
            var response = badResult!.Value as APIResponse;

            response!.ErrorMessages.Should().ContainSingle()
                     .Which.Should().Be(errorMessage);
        }

        // ── NotFoundResponse ─────────────────────────────────────────

        [Fact]
        public void NotFoundResponse_WhenCalled_ShouldReturnNotFoundStatusCode()
        {
            // Act
            var actionResult = _sut.NotFoundResponse("User not found.");

            // Assert
            var notFoundResult = actionResult.Result as NotFoundObjectResult;  // ← NotFoundObjectResult
            var response = notFoundResult!.Value as APIResponse;

            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ── InternalServerResponse ───────────────────────────────────

        [Fact]
        public void InternalServerResponse_WhenCalled_ShouldReturnInternalServerErrorStatusCode()
        {
            // Act
            var actionResult = _sut.InternalServerResponse("something went wrong");

            // Assert
            var objectResult = actionResult.Result as ObjectResult;  // ← plain ObjectResult (500 has no specific class)
            var response = objectResult!.Value as APIResponse;

            response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public void InternalServerResponse_WhenCalled_ShouldHaveNoResult()
        {
            // Act
            var actionResult = _sut.InternalServerResponse("something went wrong");

            // Assert
            var objectResult = actionResult.Result as ObjectResult;
            var response = objectResult!.Value as APIResponse;

            response!.Result.Should().BeNull();
        }
    }
}
