using System.Diagnostics;
using Yafa.Application.Common.Interfaces;

namespace Yafa.Infrastructure.Job
{
	public class BalanceExpiryJob
	{
		private const string JOB_NAME = "BalanceExpiryJob";
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogService _logService;

		public BalanceExpiryJob(IUnitOfWork unitOfWork, ILogService logService)
		{
			_unitOfWork = unitOfWork;
			_logService = logService;
		}

		public async Task CancelExpiredPartialBookingsAsync()
		{
			var stopwatch = Stopwatch.StartNew();

			try
			{
				var (result, message) = await _unitOfWork.DapperRepo.CancelExpiredPartialBookingsAsync();

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
		}
	}
}
