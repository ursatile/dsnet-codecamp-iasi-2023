using System.Net;
using Autobarn.PricingServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => {
	static Action<ListenOptions> UseCertIfAvailable(string pfxFilePath, string pfxPassword) {
		if (File.Exists(pfxFilePath)) return listen => listen.UseHttps(pfxFilePath, pfxPassword);
		return listen => listen.UseHttps();
	}
	var pfxPassword = Environment.GetEnvironmentVariable("UrsatilePfxPassword") ?? String.Empty;
	var pfxFilePath = @"D:\Dropbox\workshop.ursatile.com\workshop.ursatile.com.pfx";
	var https = UseCertIfAvailable(pfxFilePath, pfxPassword);
	options.Listen(IPAddress.Any, 5003, https);
});

builder.Services.AddGrpc();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<PricerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
