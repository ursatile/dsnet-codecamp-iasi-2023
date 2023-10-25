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
		services.AddHostedService<PricingService>();
	});

var host = builder.Build();
await host.RunAsync();
