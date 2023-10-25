using Microsoft.AspNetCore.SignalR.Client;

var url = "https://workshop.ursatile.com:5001/hub";
var hub = new HubConnectionBuilder().WithUrl(url).Build();
await hub.StartAsync();
Console.WriteLine("Hub started!");
Console.WriteLine("Press a key to send a message (or Esc to exit)...");
while (true) {
	if (Console.ReadKey(true).Key == ConsoleKey.Escape) Environment.Exit(0);
	Console.WriteLine("Sending a message");
	var message = $"Hello at {DateTime.Now}"; // <-- CHANGE THIS to something else :)
	await hub.SendAsync("TellEverybodyAboutAThingThatHappened",
		"Autobarn.Notifier", message);
	Console.WriteLine($"Sent message: {message}");
}
