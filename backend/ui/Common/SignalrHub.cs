using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ui.Common
{
	/*
	https://medium.com/@chris.stephan1996/why-when-and-how-and-to-use-signalr-ef49b5b0dc11

	Client 
		yarn add @aspnet/signalr 
	*/

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