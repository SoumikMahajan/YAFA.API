using Moq;
using Yafa.Api.Controllers.Property;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Infrastructure.Repository;

namespace Yafa.Tests.Controller.Property
{
    public class UserWishListPropertiesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDapperRepo> _mockDapperRepo;
        private readonly Mock<ILogService> _mockLogService;
        private readonly UserWishListPropertiesController _sut;

        public UserWishListPropertiesControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDapperRepo = new Mock<IDapperRepo>();
            _mockLogService = new Mock<ILogService>();

            _mockUnitOfWork
                .Setup(u => u.DapperRepo)
                .Returns(_mockDapperRepo.Object);

            _sut = new UserWishListPropertiesController(
                _mockLogService.Object,
                _mockUnitOfWork.Object,
                configuration: null!,
                new ApiResponseService()
            );
        }
    }
}
