// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function connectToSignalR() {
	console.log("Connecting to SignalR...");
	var hub = new signalR.HubConnectionBuilder().withUrl("/hub").build();
	hub.on("HeyEverybodyThisThingHappened",
		function (user, message) {
			console.log(user);
			console.log(message);
		});
	hub.start().then(function () {
		console.log("SignalR is connected!");
	}).catch(function (err) {
		console.log("SignalR went wrong!");
		console.log(err);
	});
}

$(document).ready(connectToSignalR);
