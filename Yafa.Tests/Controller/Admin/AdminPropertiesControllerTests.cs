using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Yafa.Api.Controllers.Admin;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Domain.Dtos.Admin;

namespace Yafa.Tests.Controller.Admin
{
	public class AdminPropertiesControllerTests
	{
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<IDapperRepo> _mockDapperRepo;
		private readonly Mock<IFileRepo> _mockFileService;
		private readonly Mock<ILogService> _mockLogService;
		private readonly AdminPropertiesController _sut;

		public AdminPropertiesControllerTests()
		{
			_mockUnitOfWork = new Mock<IUnitOfWork>();
			_mockDapperRepo = new Mock<IDapperRepo>();
			_mockFileService = new Mock<IFileRepo>();
			_mockLogService = new Mock<ILogService>();

			_mockUnitOfWork.Setup(u => u.DapperRepo).Returns(_mockDapperRepo.Object);

			_sut = new AdminPropertiesController(_mockLogService.Object, _mockUnitOfWork.Object, configuration: null!, new ApiResponseService(), _mockFileService.Object);
		}

		// ── BlockPropertyDates ───────────────────────────────────────
		[Fact]
		public async Task BlockPropertyDates_WhenRequestIsValid_ShouldReturnSuccessResponse()
		{
			// Arrange — Feature 1: Setup + ReturnsAsync with tuple
			var request = new BlockPropRequestDTO
			{
				PropertyPublicId = Guid.NewGuid(),
				BlockDates = "2026-05-01,2026-05-02",
				BlockReason = "Maintenance",
				CreatedBy = "admin-001"
			};

			_mockDapperRepo.Setup(d => d.AddBlockPropertyDatesAsync(request)).ReturnsAsync((1, "Dates blocked successfully."));

			// Act
			var result = await _sut.BlockPropertyDates(request);

			// Assert
			var okResult = result.Result as OkObjectResult;
			var response = okResult!.Value as APIResponse;

			response!.IsSuccess.Should().BeTrue();
		}

		[Fact]
		public async Task BlockPropertyDates_WhenDbFails_ShouldReturnInternalServerError()
		{
			// Arrange — Feature 2: It.IsAny — don't care about the exact request
			_mockDapperRepo.Setup(d => d.AddBlockPropertyDatesAsync(It.IsAny<BlockPropRequestDTO>())).ReturnsAsync((0, "Failed to block dates."));

			// Act
			var result = await _sut.BlockPropertyDates(new BlockPropRequestDTO());

			// Assert
			var serverError = result.Result as ObjectResult;
			var response = serverError!.Value as APIResponse;

			response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}


		// ── UnBlockPropertyDates ─────────────────────────────────────

		[Fact]
		public async Task UnBlockPropertyDates_WhenRequestIsValid_ShouldReturnSuccessResponse()
		{
			// Arrange — Feature 2: It.IsAny<Guid>()
			var validId = Guid.NewGuid();

			_mockDapperRepo.Setup(d => d.UnBlockPropertyDatesDatesAsync(It.IsAny<Guid>())).ReturnsAsync((1, "Date unblocked successfully."));

			// Act
			var result = await _sut.UnBlockPropertyDates(validId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			var response = okResult!.Value as APIResponse;

			response!.IsSuccess.Should().BeTrue();
		}

		[Fact]
		public async Task UnBlockPropertyDates_WhenDbFails_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockDapperRepo.Setup(d => d.UnBlockPropertyDatesDatesAsync(It.IsAny<Guid>())).ReturnsAsync((0, "Unblock failed."));

			// Act
			var result = await _sut.UnBlockPropertyDates(Guid.NewGuid());

			// Assert
			var serverError = result.Result as ObjectResult;
			var response = serverError!.Value as APIResponse;

			response!.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		}

		// ── GetPropertyBlockedDates ──────────────────────────────────

		[Fact]
		public async Task GetPropertyBlockedDates_WhenPropertyIdIsEmpty_ShouldReturnBadRequest()
		{
			// Arrange — no mock setup needed, guard clause fires first

			// Act
			var result = await _sut.GetPropertyBlockedDates(Guid.Empty);

			// Assert
			var badResult = result.Result as BadRequestObjectResult;
			var response = badResult!.Value as APIResponse;

			response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task GetPropertyBlockedDates_WhenPropertyExists_ShouldReturnOkResponse()
		{
			// Arrange
			var propertyId = Guid.NewGuid();
			var fakeBlockDates = new List<BlockPropDatesDetailsDTO>
			{
				new BlockPropDatesDetailsDTO { BlockReason = "Maintenance" },
				new BlockPropDatesDetailsDTO { BlockReason = "Owner stay"  }
			};

			_mockDapperRepo.Setup(d => d.BlockPropDatesDetailsAsync(propertyId)).ReturnsAsync(fakeBlockDates);

			// Act
			var result = await _sut.GetPropertyBlockedDates(propertyId);

			// Assert
			var okResult = result.Result as OkObjectResult;
			var response = okResult!.Value as APIResponse;

			response!.IsSuccess.Should().BeTrue();
		}

		[Fact]
		public async Task GetPropertyBlockedDates_WhenDbThrows_ShouldThrowException()
		{
			// Arrange
			var propertyId = Guid.NewGuid();

			_mockDapperRepo.Setup(d => d.BlockPropDatesDetailsAsync(propertyId)).ThrowsAsync(new InvalidOperationException("Somthing went wrong in DB"));

			// Act
			var act = async () => await _sut.GetPropertyBlockedDates(propertyId);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
					 .WithMessage("Somthing went wrong in DB");
		}

		// ── GetAllFinalSubmitProperties ──────────────────────────────
		//[Fact]
		//public async Task GetAllFinalSubmitProperties_WhenPropertiesExist_ShouldReturnOkResponse()
		//{
		//	// Arrange
		//	var fakeProperties = new List<FinalSubmitPropertyResponceDTO>
		//	{
		//		new FinalSubmitPropertyResponceDTO { PropertyName = "Beach Villa" },
		//		new FinalSubmitPropertyResponceDTO { PropertyName = "City Loft"   }
		//	};

		//	_mockDapperRepo.Setup(d => d.GetAllFinalSubmitPropertiesAsync()).ReturnsAsync(fakeProperties);

		//	// Act
		//	var result = await _sut.GetAllFinalSubmitProperties();

		//	// Assert
		//	var okResult = result.Result as OkObjectResult;
		//	var response = okResult!.Value as APIResponse;

		//	response!.IsSuccess.Should().BeTrue();
		//}

		// ── Feature 3: Exception testing ─────────────────────────────

		//[Fact]
		//public async Task GetAllFinalSubmitProperties_WhenDbThrows_ShouldThrowException()
		//{
		//	// Arrange — Feature 3: Throws
		//	_mockDapperRepo.Setup(d => d.GetAllFinalSubmitPropertiesAsync()).ThrowsAsync(new InvalidOperationException("Somthing went wrong in DB"));

		//	// Act
		//	var act = async () => await _sut.GetAllFinalSubmitProperties();

		//	// Assert — the exception bubbles up (middleware catches it in real app)
		//	await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Somthing went wrong in DB");
		//}

		[Fact]
		public async Task BlockPropertyDates_WhenCalled_ShouldCallAddBlockPropertyDatesAsyncOnce()
		{
			// Arrange
			_mockDapperRepo
				.Setup(d => d.AddBlockPropertyDatesAsync(It.IsAny<BlockPropRequestDTO>()))
				.ReturnsAsync((1, "Blocked."));

			// Act
			await _sut.BlockPropertyDates(new BlockPropRequestDTO());

			// Assert — Feature 4: Verify the DB method was actually called
			_mockDapperRepo.Verify(
				d => d.AddBlockPropertyDatesAsync(It.IsAny<BlockPropRequestDTO>()),
				Times.Once);
		}

	}
}
