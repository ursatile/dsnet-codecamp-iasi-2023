using Autobarn.Messages;
using Autobarn.Pricing;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autobarn.PricingClient; 

class PricingService : IHostedService {
	private readonly IBus bus;
	private readonly Pricer.PricerClient pricer;
	private readonly ILogger<PricingService> logger;

	public PricingService(IBus bus,
		Pricer.PricerClient pricer,
		ILogger<PricingService> logger) {
		this.bus = bus;
		this.pricer = pricer;
		this.logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken) {
		logger.LogInformation("Subscribing to NewVehicleMessage");
		await bus.PubSub.SubscribeAsync<NewVehicleMessage>(
			$"autobarn.auditlog@{Environment.MachineName}",
			Handle,
			cancellationToken
		);
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		return Task.CompletedTask;
	}

	public async Task Handle(NewVehicleMessage message) {
		logger.LogInformation("NewVehicleMessage: {message}", message);
		var request = new PriceRequest {
			Year = message.Year,
			Color = message.Color,
			Make = message.Make,
			Model = message.Model
		};
		var priceReply = await pricer.GetPriceAsync(request);
		logger.LogInformation("{price} {currencyCode}", priceReply.Price, priceReply.CurrencyCode);
		// 3: ???
	}
}
