using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ui.Common
{
	public interface IHubClient
	{
		Task BroadcastMessage(MessageInstance msg);
	}

	public class MessageInstance
	{
		public string Timestamp { get; set; }
		public string From { get; set; }
		public string Message { get; set; }
	}

	public class SignalrHub : Hub<IHubClient>
	{
		public async Task BroadcastMessage(MessageInstance msg)
		{
			await Clients.All.BroadcastMessage(msg);
		}
	}
}