using FirebaseAdmin.Messaging;
using Yafa.Application.Common.Interfaces;

namespace Yafa.Infrastructure.Repository;

public class FirebaseService : IFirebaseService
{
	private const int FCM_BATCH_LIMIT = 500;
	public async Task<string> SendAsync(string token, string title, string body, Dictionary<string, string>? data = null)
	{
		var message = new Message
		{
			Token = token,
			Notification = new Notification
			{
				Title = title,
				Body = body,
			},
			Data = data ?? new Dictionary<string, string>()
		};

		var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
		return response;
	}

	public async Task<(int Success, List<string> FailedTokens)> SendBulkAsync(List<string> tokens, string title, string body, Dictionary<string, string>? data = null)
	{
		int totalSuccess = 0;
		var allFailedTokens = new List<string>();

		var batches = tokens
		.Select((token, i) => (token, i))
		.GroupBy(x => x.i / FCM_BATCH_LIMIT)
		.Select(g => g.Select(x => x.token).ToList())
		.ToList();

		foreach (var batch in batches)
		{
			var message = new MulticastMessage
			{
				Tokens = batch,
				Notification = new Notification { Title = title, Body = body },
				Data = data ?? new Dictionary<string, string>()
			};

			var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

			totalSuccess += response.SuccessCount;

			// Collect invalid tokens for DB cleanup
			for (int i = 0; i < response.Responses.Count; i++)
			{
				if (!response.Responses[i].IsSuccess)
				{
					var errorCode = response.Responses[i].Exception?.MessagingErrorCode;

					// Only deactivate tokens Firebase says are permanently invalid
					// NOT network errors — those are temporary
					if (errorCode == MessagingErrorCode.Unregistered ||
						errorCode == MessagingErrorCode.InvalidArgument ||
						errorCode == MessagingErrorCode.SenderIdMismatch)
					{
						allFailedTokens.Add(batch[i]);
					}
				}
			}
		}

		return (totalSuccess, allFailedTokens);


	}

	public int GetBatchCount(int tokenCount) =>
		(int)Math.Ceiling((double)tokenCount / FCM_BATCH_LIMIT);
}
