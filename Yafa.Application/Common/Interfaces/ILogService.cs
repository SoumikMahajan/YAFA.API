namespace Yafa.Application.Common.Interfaces
{
    public interface ILogService
    {
        Task LogExceptionErrorAsync(Exception ex);
        Task LogCustomAsync(string message, string logType = "INFO");
		Task LogJobAsync(string jobName, string status, string message, TimeSpan? duration = null);
	}
}
