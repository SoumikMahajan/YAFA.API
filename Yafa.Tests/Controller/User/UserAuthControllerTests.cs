using Moq;
using Yafa.Api.Controllers.User;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Infrastructure.Repository;

namespace Yafa.Tests.Controller.User
{
    public class UserAuthControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IDapperRepo> _mockDapperRepo;
        private readonly Mock<IEmailRepo> _mockEmailRepo;
        private readonly Mock<IFileRepo> _mockFileService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ILogService> _mockLogService;
        private readonly UserAuthController _sut;

        public UserAuthControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockDapperRepo = new Mock<IDapperRepo>();
            _mockEmailRepo = new Mock<IEmailRepo>();
            _mockFileService = new Mock<IFileRepo>();
            _mockJwtService = new Mock<IJwtService>();
            _mockLogService = new Mock<ILogService>();

            _mockUnitOfWork
                .Setup(u => u.DapperRepo)
                .Returns(_mockDapperRepo.Object);

            _mockUnitOfWork
                .Setup(u => u.EmailRepo)
                .Returns(_mockEmailRepo.Object);

            //_sut = new UserAuthController(
            //    _mockLogService.Object,
            //    _mockUnitOfWork.Object,
            //    configuration: null!,
            //    new ApiResponseService(),
            //    _mockJwtService.Object,
            //    _mockFileService.Object
            //);
        }
    }
}
