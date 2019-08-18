using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ui.Common;

namespace ui.Controllers
{
	[Route("api/[controller]")]
	public class TestController : Controller
	{
		private IHubContext<SignalrHub, IHubClient> hub;

		public TestController(IHubContext<SignalrHub, IHubClient> hub)
		{
			this.hub = hub;
		}

		[HttpPost]
		public async Task<string> Post([FromBody]MessageInstance msg)
		{
			var retMessage = string.Empty;
			try
			{
				msg.Timestamp = DateTime.UtcNow.ToString();
				await this.hub.Clients.All.BroadcastMessage(msg);
				retMessage = "Success";
			}
			catch (Exception e)
			{
				retMessage = e.ToString();
			}
			return retMessage;
		}

		// https://localhost:5001/api/data/check?keyId=233&lockid=34434&dateTime=2010-12-09T08:00:00.000Z	
	}
}