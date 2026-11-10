using Moq;
using Yafa.Api.Controllers.Admin;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Infrastructure.Repository;

namespace Yafa.Tests.Controller.Admin
{
    public class AdminAuthControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDapperRepo> _mockDapperRepo;
        private readonly Mock<IFileRepo> _mockFileService;
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AdminAuthController _sut;

        public AdminAuthControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDapperRepo = new Mock<IDapperRepo>();
            _mockFileService = new Mock<IFileRepo>();
            _mockLogService = new Mock<ILogService>();
            _mockJwtService = new Mock<IJwtService>();

            _mockUnitOfWork
                .Setup(u => u.DapperRepo)
                .Returns(_mockDapperRepo.Object);

            _sut = new AdminAuthController(
                _mockLogService.Object,
                _mockUnitOfWork.Object,
                configuration: null!,
                new ApiResponseService(),
                _mockJwtService.Object,
                _mockFileService.Object
            );
        }
    }
}
