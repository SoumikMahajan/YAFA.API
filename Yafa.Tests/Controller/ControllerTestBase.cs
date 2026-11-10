using Moq;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;

namespace Yafa.Tests.Controller
{
	public abstract class ControllerTestBase
	{
		// ── Shared mocks every controller needs ───────────────────
		protected readonly Mock<IUnitOfWork> MockUnitOfWork;
		protected readonly Mock<IDapperRepo> MockDapperRepo;
		protected readonly Mock<ILogService> MockLogService;

		// ── Shared real services (no dependencies, safe to reuse) ─
		protected readonly ApiResponseService ResponseService;

		protected ControllerTestBase()
		{
			MockUnitOfWork = new Mock<IUnitOfWork>();
			MockDapperRepo = new Mock<IDapperRepo>();
			MockLogService = new Mock<ILogService>();
			ResponseService = new ApiResponseService();

			// Always wire DapperRepo into UnitOfWork
			MockUnitOfWork
				.Setup(u => u.DapperRepo)
				.Returns(MockDapperRepo.Object);
		}
	}
}
