using Autobarn.Messages;
using Autobarn.Pricing;
using Autobarn.PricingClient;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder()
	.ConfigureLogging((_, logging) => {
		logging.ClearProviders();
		logging.AddConsole();
	})
	.ConfigureServices((context, services) => {
		using var channel
			= GrpcChannel.ForAddress("https://workshop.ursatile.com:5003");
		var client = new Pricer.PricerClient(channel);
		var amqp = context.Configuration.GetConnectionString("rabbitmq_autobarn");
		var bus = RabbitHutch.CreateBus(amqp);
		services.AddSingleton(bus);
		services.AddSingleton(client);
		services.AddHostedService<PricingService>();
	});

var host = builder.Build();
await host.RunAsync();
#pragma warning restore CS4014

//using var scope = host.Services.CreateScope();
//var bus = scope.ServiceProvider.GetService<IBus>();
//var client = scope.ServiceProvider.GetService<Pricer.PricerClient>();
//var logger = scope.ServiceProvider.GetService<ILogger<PricingService>>();
//var pricer = new PricingService(bus, client, logger);
//while (true) {
//	Console.WriteLine("Press a key or Esc to exit");
//	var key = Console.ReadKey(true);
//	if (key.Key == ConsoleKey.Escape) Environment.Exit(0);
//	Console.WriteLine("Testing pricing server...");
//	var m = new NewVehicleMessage {
//		Color = "Blue",
//		Make = "Volkswagen",
//		Model = "Beetle",
//		Year = 1987,
//		ListedAt = DateTimeOffset.UtcNow
//	};
//	await pricer.Handle(m);
//}
