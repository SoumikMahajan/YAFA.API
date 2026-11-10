namespace Yafa.Application.Common.Interfaces
{
    public interface IFirebaseService
    {
		Task<string> SendAsync(string token, string title, string body, Dictionary<string, string>? data = null);
		Task<(int Success, List<string> FailedTokens)> SendBulkAsync(List<string> tokens, string title, string body, Dictionary<string, string>? data = null);
		int GetBatchCount(int count);
	}
}
