using Autobarn.Messages;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autobarn.AuditLog; 

class AuditLogService : IHostedService {
	private readonly IBus bus;
	private readonly ILogger<AuditLogService> logger;

	public AuditLogService(IBus bus, ILogger<AuditLogService> logger) {
		this.bus = bus;
		this.logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken) {
		logger.LogInformation("Subscribing to NewVehicleMessage");
		await bus.PubSub.SubscribeAsync<NewVehicleMessage>(
			"autobarn.auditlog",
			Handle,
			cancellationToken
		);
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		return Task.CompletedTask;
	}

	public void Handle(NewVehicleMessage message) {
		logger.LogInformation("NewVehicleMessage: {message}", message);
	}
}
