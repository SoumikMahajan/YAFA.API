using System.Diagnostics;
using Yafa.Application.Common.Interfaces;

namespace Yafa.Infrastructure.Job
{
	public class HoldExpiryJob
	{
		private const string JOB_NAME = "HoldExpiryJob";
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogService _logService;

		public HoldExpiryJob(IUnitOfWork unitOfWork, ILogService logService)
		{
			_unitOfWork = unitOfWork;
			_logService = logService;
		}

		public async Task ReleaseExpiredHoldsAsync()
		{
			var stopwatch = Stopwatch.StartNew();

			try
			{
				var (result, message) = await _unitOfWork.DapperRepo.ReleaseExpiredHoldsAsync();

				stopwatch.Stop();

				if (result > 0)
				{
					await _logService.LogJobAsync(JOB_NAME, "COMPLETED", message, stopwatch.Elapsed);
					return;
				}
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				await _logService.LogExceptionErrorAsync(ex);
				await _logService.LogJobAsync(JOB_NAME, "FAILED", $"Error: {ex.Message}", stopwatch.Elapsed);
			}

			var extStopwatch = Stopwatch.StartNew();

			try
			{
				var (extResult, extMessage) = await _unitOfWork.DapperRepo.ReleaseExpiredExtensionHoldsAsync();

				extStopwatch.Stop();
				await _logService.LogJobAsync(JOB_NAME, "COMPLETED", $"[ExtensionHolds] {extMessage}", extStopwatch.Elapsed);
			}
			catch (Exception ex)
			{
				extStopwatch.Stop();
				await _logService.LogExceptionErrorAsync(ex);
				await _logService.LogJobAsync(JOB_NAME, "FAILED", $"[ExtensionHolds] Error: {ex.Message}", extStopwatch.Elapsed);
			}
		}

	}
}
