using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Yafa.Api.Controllers.Admin;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Tests.Controller.Admin
{
	public class AdminUserControllerTests : ControllerTestBase
	{
		private readonly AdminUserController _sut;

		public AdminUserControllerTests()
		{
			//_sut = new AdminUserController(
			//	MockLogService.Object,
			//	MockUnitOfWork.Object,
			//	configuration: null!,
			//	ResponseService
			//);
		}

		//[Fact]
		//public async Task GetAllUserList_WhenUsersExist_ShouldReturnOkResponse()
		//{
		//	// ── ARRANGE ─────────────────────────────────────────────────────
		//	// Everything the test needs before the action is called.
		//	// We are telling the mock: "when GetAllUserListAsync is called,
		//	// pretend the database returned this list"

		//	var fakeUsers = new List<UserDetailsDTO>
		//	{
		//		new UserDetailsDTO { UserId = "u1", FirstName = "Ahmed" },
		//		new UserDetailsDTO { UserId = "u2", FirstName = "Sara"  }
		//	};

		//	MockDapperRepo.Setup(d => d.GetAllUserListAsync()).ReturnsAsync(fakeUsers);

		//	// ── ACT ──────────────────────────────────────────────────────────
		//	// Call the real controller method. Always exactly one line.

		//	var result = await _sut.GetAllUserList();

		//	// ── ASSERT ───────────────────────────────────────────────────────
		//	// Verify the outcome. One thing per test.

		//	var okResult = result.Result as OkObjectResult;
		//	var response = okResult!.Value as APIResponse;

		//	response!.IsSuccess.Should().BeTrue();
		//}

		////[Fact]
		////public async Task GetAllUserList_WhenUsersExist_ShouldReturnOkStatusCode()
		////{
		////    // Arrange
		////    var fakeUsers = new List<UserDetailsDTO>
		////    {
		////        new UserDetailsDTO { UserId = "u1", FirstName = "Ahmed" }
		////    };

		////    _mockDapperRepo.Setup(d => d.GetAllUserListAsync()).ReturnsAsync(fakeUsers);

		////    // Act
		////    var result = await _sut.GetAllUserList();

		////    // Assert
		////    var okResult = result.Result as OkObjectResult;
		////    var response = okResult!.Value as APIResponse;

		////    response!.StatusCode.Should().Be(HttpStatusCode.OK);
		////}

		//[Fact]
		//public async Task GetAllUserList_WhenNoUsersFound_ShouldReturnSuccessWithMessage()
		//{
		//	// Arrange — null simulates "no users" from DB
		//	MockDapperRepo
		//		.Setup(d => d.GetAllUserListAsync())
		//		.ReturnsAsync((IEnumerable<UserDetailsDTO>)null!);

		//	// Act
		//	var result = await _sut.GetAllUserList();

		//	// Assert
		//	var okResult = result.Result as OkObjectResult;
		//	var response = okResult!.Value as APIResponse;

		//	response!.IsSuccess.Should().BeTrue();
		//	response.Result.Should().Be("No user found!");
		//}

		//[Fact]
		//public async Task GetUserDetailsById_WhenUserIdIsEmpty_ShouldReturnBadRequest()
		//{
		//	// Arrange
		//	var emptyId = string.Empty;

		//	// Act
		//	var result = await _sut.GetUserDetailsById(emptyId);

		//	// Assert
		//	var badResult = result.Result as BadRequestObjectResult;
		//	var response = badResult!.Value as APIResponse;

		//	response!.IsSuccess.Should().BeFalse();
		//}

		//[Fact]
		//public async Task GetUserDetailsById_WhenUserIdIsEmpty_ShouldReturnBadRequestStatusCode()
		//{
		//	// Act
		//	var result = await _sut.GetUserDetailsById(string.Empty);

		//	// Assert
		//	var badResult = result.Result as BadRequestObjectResult;
		//	var response = badResult!.Value as APIResponse;

		//	response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		//}

		//[Fact]
		//public async Task GetUserDetailsById_WhenUserExists_ShouldReturnOkWithUserDetails()
		//{
		//	// Arrange
		//	var userId = "user-abc-123";
		//	var fakeUser = new UserDetailsDTO
		//	{
		//		UserId = userId,
		//		FirstName = "Ahmed",
		//		LastName = "Hassan"
		//	};

		//	MockDapperRepo
		//		.Setup(d => d.GetUserDetailsByIdAsync(userId))
		//		.ReturnsAsync(fakeUser);

		//	// Act
		//	var result = await _sut.GetUserDetailsById(userId);

		//	// Assert
		//	var okResult = result.Result as OkObjectResult;
		//	var response = okResult!.Value as APIResponse;

		//	response!.IsSuccess.Should().BeTrue();
		//}

		//[Fact]
		//public async Task GetUserDetailsById_WhenUserNotFound_ShouldReturnNotFound()
		//{
		//	// Arrange
		//	MockDapperRepo.Setup(d => d.GetUserDetailsByIdAsync(It.IsAny<string>())).ReturnsAsync((UserDetailsDTO)null!);

		//	// Act
		//	var result = await _sut.GetUserDetailsById("nonexistent-id");

		//	// Assert
		//	var serverError = result.Result as ObjectResult;
		//	var response = serverError!.Value as APIResponse;

		//	response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
		//}
	}
}
