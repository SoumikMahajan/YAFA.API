using Moq;
using Yafa.Api.Controllers.Property;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Infrastructure.Repository;

namespace Yafa.Tests.Controller.Property
{
    public class UserPropertiesControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDapperRepo> _mockDapperRepo;
        private readonly Mock<IFileRepo> _mockFileService;
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<IFirebaseService> _mockFirebaseService;
        private readonly UserPropertiesController _sut;


        public UserPropertiesControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDapperRepo = new Mock<IDapperRepo>();
            _mockFileService = new Mock<IFileRepo>();
            _mockLogService = new Mock<ILogService>();
            _mockFirebaseService = new Mock<IFirebaseService>();

            _mockUnitOfWork
                .Setup(u => u.DapperRepo)
                .Returns(_mockDapperRepo.Object);

            _sut = new UserPropertiesController(
                _mockLogService.Object,
                _mockUnitOfWork.Object,
                configuration: null!,
                new ApiResponseService(),
                _mockFileService.Object,
                _mockFirebaseService.Object
            );
        }
    }
}
