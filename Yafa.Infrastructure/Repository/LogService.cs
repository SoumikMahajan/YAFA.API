using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Yafa.Application.Common.Interfaces;

namespace Yafa.Infrastructure.Repository
{
	public class LogService : ILogService
	{
		private readonly ILogger<LogService> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private static readonly SemaphoreSlim _logSemaphore = new(1, 1);
		private const int MaxWaitTimeMs = 5000;
		private const string LogDirectory = "YafaLogs";
		public LogService(ILogger<LogService> logger, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task LogExceptionErrorAsync(Exception ex)
		{
			var logFile = GetLogFilePath();
			EnsureDirectoryExists(LogDirectory);

			if (!await _logSemaphore.WaitAsync(MaxWaitTimeMs))
			{
				_logger.LogWarning("LogService timeout (LogExceptionErrorAsync): could not acquire file lock.");
				return;
			}
			try
			{
				if (!File.Exists(logFile))
				{
					await using var _ = File.Create(logFile);
				}
				await using var txtWriter = new StreamWriter(logFile, append: true);
				await LogAsync(ex, txtWriter);
			}
			finally
			{
				_logSemaphore.Release();
			}
		}

		private async Task LogAsync(Exception ex, TextWriter txtWriter)
		{
			var userId = GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User!);

			await txtWriter.WriteLineAsync($"\r\n[ERROR] {DateTime.Now:D} {DateTime.Now:T}");
			await txtWriter.WriteLineAsync($"  :[LoggedInUser]: {userId}");
			await txtWriter.WriteLineAsync($"  :[Message]: {ex.Message}");

			var stackTrace = ex.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			await txtWriter.WriteLineAsync($"  :[StackTrace]: {string.Join(Environment.NewLine, stackTrace!)}");
			await txtWriter.WriteLineAsync("---------------------------------------------------------------------------------------------");

		}

		public async Task LogCustomAsync(string message, string logType = "INFO")
		{
			var logFile = GetLogFilePath();
			EnsureDirectoryExists(LogDirectory);

			if (!await _logSemaphore.WaitAsync(MaxWaitTimeMs))
			{
				_logger.LogWarning("LogService timeout (LogCustomAsync): could not acquire file lock.");
				return;
			}
			try
			{
				if (!File.Exists(logFile))
				{
					await using var _ = File.Create(logFile);
				}

				await using var txtWriter = new StreamWriter(logFile, append: true);
				var userId = GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User!);

				await txtWriter.WriteLineAsync($"\r\n[{logType}]: {DateTime.Now:D} {DateTime.Now:T}");
				await txtWriter.WriteLineAsync($" :[LoggedInUser]: {userId}");
				await txtWriter.WriteLineAsync($" :[Message]: {message}");
				await txtWriter.WriteLineAsync("------------------------------------------------------------");
			}
			finally
			{
				_logSemaphore.Release();
			}

		}

		public async Task LogJobAsync(string jobName, string status, string message, TimeSpan? duration = null)
		{
			var logFile = GetLogFilePath();
			EnsureDirectoryExists(LogDirectory);

			if (!await _logSemaphore.WaitAsync(MaxWaitTimeMs))
			{
				_logger.LogWarning("LogService timeout (LogJobAsync): could not acquire file lock.");
				return;
			}

			try
			{
				if (!File.Exists(logFile))
				{
					await using var _ = File.Create(logFile);
				}

				await using var txtWriter = new StreamWriter(logFile, append: true);

				await txtWriter.WriteLineAsync($"\r\n[JOB:{status}] {DateTime.UtcNow:D} {DateTime.UtcNow:T} UTC");
				await txtWriter.WriteLineAsync($"  :[Job]: {jobName}");
				await txtWriter.WriteLineAsync($"  :[Message]: {message}");

				if (duration.HasValue)
					await txtWriter.WriteLineAsync($"  :[Duration]: {duration.Value.TotalSeconds:F2}s");

				await txtWriter.WriteLineAsync("---------------------------------------------------------------------------------------------");
			}
			finally
			{
				_logSemaphore.Release();
			}
		}

		private void EnsureDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		private string GetLogFilePath()
		{
			return Path.Combine(LogDirectory, $"Log-{DateTime.Now:MM-dd-yyyy}.txt");
		}

		private string GetUserIdFromClaims(ClaimsPrincipal user)
		{
			var userIdClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

			return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
		}


	}
}
