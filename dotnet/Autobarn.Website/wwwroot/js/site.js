// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function connectToSignalR() {
	console.log("Connecting to SignalR...");
	var hub = new signalR.HubConnectionBuilder().withUrl("/hub").build();
	hub.on("HeyEverybodyThisThingHappened", displayNewVehicleNotification);
	hub.start().then(function () {
		console.log("SignalR is connected!");
	}).catch(function (err) {
		console.log("SignalR went wrong!");
		console.log(err);
	});
}

function displayNewVehicleNotification(user, json) {
	console.log(json);
	var data = JSON.parse(json);
	// { "Price": 105250, "CurrencyCode": "RON", "Registration": "ABC125", "Make": "HONDA", "Model": "CIVIC", "Color": "white", "Year": 2005, "ListedAt": "2023-10-25T12:58:36.3904002+00:00" }
	var $alert = $(`<div><h4>New vehicle!</h4>
${data.Make} ${data.Model} (${data.Color}, ${data.Year}<br />
Price: ${data.Price} ${data.CurrencyCode}<br />
<a href="/vehicles/details/${data.Registration}">click for more info...</a>
</div>`);
	var div = $("#signalr-notifications");
	$alert.css("background-color", data.Color);
	div.prepend($alert);
	window.setTimeout(function() {
		$alert.fadeOut(2000,
			function() {
				$alert.remove();
			});
	}, 5000);
}

$(document).ready(connectToSignalR);
