using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Autobarn.Website.Hubs {
	public class AutobarnHub : Hub {
		private readonly ILogger<AutobarnHub> logger;

		public AutobarnHub(ILogger<AutobarnHub> logger) {
			this.logger = logger;
		}
		public async Task TellEverybodyAboutAThingThatHappened(string user, string message) {
			logger.LogInformation("Received a message: {message}", message);
			await Clients.All.SendAsync("HeyEverybodyThisThingHappened", user, message);
		}
	}
}
