using Autobarn.Messages;
using Autobarn.Pricing;
using Autobarn.PricingClient;
using EasyNetQ;
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
		var amqp = context.Configuration.GetConnectionString("rabbitmq_autobarn");
		var bus = RabbitHutch.CreateBus(amqp);
		services.AddSingleton(bus);
		var uri = context.Configuration["AutobarnPricingServerUrl"];
		services.AddGrpcClient<Pricer.PricerClient>(options => options.Address = new Uri(uri));
		services.AddHostedService<PricingService>();
	});

var host = builder.Build();
using var scope = host.Services.CreateScope();
var bus = scope.ServiceProvider.GetService<IBus>();
var client = scope.ServiceProvider.GetService<Pricer.PricerClient>();
var logger = scope.ServiceProvider.GetService<ILogger<PricingService>>();
var pricer = new PricingService(bus, client, logger);

host.RunAsync();
#pragma warning restore CS4014


while (true) {
	Console.WriteLine("Press a key or Esc to exit");
	var key = Console.ReadKey(true);
	if (key.Key == ConsoleKey.Escape) Environment.Exit(0);
	Console.WriteLine("Testing pricing server...");
	var m = new NewVehicleMessage {
		Color = "Blue",
		Make = "Volkswagen",
		Model = "Beetle",
		Year = 1987,
		ListedAt = DateTimeOffset.UtcNow
	};
	await pricer.Handle(m);
}
