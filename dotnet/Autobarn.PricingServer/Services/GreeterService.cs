using Autobarn.Pricing;
using Grpc.Core;

namespace Autobarn.PricingServer.Services;

public class PricerService : Pricer.PricerBase {
	private readonly ILogger<PricerService> logger;
	public PricerService(ILogger<PricerService> logger) {
		this.logger = logger;
	}

	public override Task<PriceReply> GetPrice(PriceRequest request, ServerCallContext context) {
		logger.LogInformation("Received a request: {request}", request);
		var reply = new PriceReply {
			Price = 123456,
			CurrencyCode = "RON"
		};
		logger.LogInformation("Reply: {reply}", reply);
		return Task.FromResult(reply);
	}
}
