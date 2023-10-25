using System.Text.Json.Serialization;
using Autobarn.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

var builder = Host.CreateDefaultBuilder()
	.ConfigureLogging((_, logging) => {
		logging.ClearProviders();
		logging.AddConsole();
	})
	.ConfigureServices((context, services) => {
		var hubUrl = context.Configuration["AutobarnHubUrl"];
		var hub = new HubConnectionBuilder().WithUrl(hubUrl).Build();
		services.AddSingleton(hub);
		var amqp = context.Configuration.GetConnectionString("rabbitmq_autobarn");
		var bus = RabbitHutch.CreateBus(amqp);
		services.AddSingleton(bus);
		services.AddHostedService<NotifierService>();
	});

var host = builder.Build();
await host.RunAsync();

public class NotifierService : IHostedService {
	private readonly HubConnection hub;
	private readonly IBus bus;
	private readonly ILogger<NotifierService> logger;

	public NotifierService(HubConnection hub, IBus bus, ILogger<NotifierService> logger) {
		this.hub = hub;
		this.bus = bus;
		this.logger = logger;
	}
	public async Task StartAsync(CancellationToken cancellationToken) {
		await hub.StartAsync(cancellationToken);
		logger.LogInformation("Notifier connected to SignalR Hub.");
		await bus.PubSub.SubscribeAsync<NewVehiclePriceMessage>("autobarn.notifier",
			HandleNewVehiclePriceMessage);
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		await hub.StopAsync(cancellationToken);
	}

	public async Task HandleNewVehiclePriceMessage(NewVehiclePriceMessage m) {
		logger.LogInformation("Received NewVehiclePriceMessage {m}", m);
		var json = JsonConvert.SerializeObject(m);
		await hub.SendAsync("TellEverybodyAboutAThingThatHappened", "autobarn.notifier", json);
		logger.LogInformation("Sent to SignalR!");
	}
}


