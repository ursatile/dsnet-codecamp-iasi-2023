using Autobarn.AuditLog;
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
		services.AddHostedService<AuditLogService>();
	});

var host = builder.Build();
await host.RunAsync();
