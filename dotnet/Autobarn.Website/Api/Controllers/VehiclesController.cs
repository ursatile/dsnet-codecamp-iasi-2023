using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Microsoft.AspNetCore.Mvc;
using Autobarn.Website.Models;
using Castle.Core.Logging;
using EasyNetQ;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace Autobarn.Website.Api.Controllers;

[Route("api/[controller]")]

[ApiController]
public class VehiclesController : ControllerBase {
	private readonly IAutobarnDatabase db;
	private readonly IBus bus;
	private readonly ILogger<VehiclesController> logger;

	public VehiclesController(
		IAutobarnDatabase db,
		IBus bus,
		ILogger<VehiclesController> logger) {
		this.db = db;
		this.bus = bus;
		this.logger = logger;
	}

	private const int PAGE_SIZE = 20;
	// GET: api/vehicles
	[HttpGet]
	public object Get(int index = 0) {
		var total = db.CountVehicles();
		var items = db.ListVehicles().Skip(index).Take(PAGE_SIZE);
		dynamic _links = new ExpandoObject();
		_links.self = new {
			href = $"/api/vehicles?index={index}"
		};
		if (index > 0) {
			_links.previous = new {
				href = $"/api/vehicles?index={index - PAGE_SIZE}"
			};
		}

		if (index < total) {
			_links.next = new {
				href = $"/api/vehicles?index={index + PAGE_SIZE}"
			};
		}
		return new {
			_links,
			items,
		};
	}


	[HttpGet("{id}")]
	public object Get(string id) {
		logger.LogInformation("GET /api/vehicles/{id}", id);
		var vehicle = db.FindVehicle(id);
		if (vehicle == default) return NotFound($"Sorry, there's no vehicle matching {id}");
		var result = vehicle.ToDynamic();
		result._links = new {
			model = new {
				href = $"/api/models/{vehicle.ModelCode}"
			},
			self = new {
				href = $"/api/vehicles/{id}"
			}
		};
		return result;
	}


	// POST api/vehicles
	[HttpPost]
	public async Task<IActionResult> Post([FromBody] VehicleDto dto) {
		var existing = db.FindVehicle(dto.Registration);
		if (existing != default) {
			logger.LogInformation("409 Conflict: POST /api/vehicles: {vehicle}", dto);
			return Conflict(
				$"There is already a vehicle with registration {dto.Registration} in our system and you can't sell the same car twice.");
		}
		var vehicleModel = db.FindModel(dto.ModelCode);
		var vehicle = new Vehicle {
			Registration = dto.Registration,
			Color = dto.Color,
			Year = dto.Year,
			VehicleModel = vehicleModel
		};

		db.CreateVehicle(vehicle);
		await PublishNewVehicleNotification(vehicle);

		logger.LogInformation("201 Created: POST /api/vehicles: {vehicle}", dto);
		return Created($"/api/vehicles/{vehicle.Registration}", vehicle);
	}

	private async Task PublishNewVehicleNotification(Vehicle vehicle) {
		
		var message = new NewVehicleMessage {
			Registration = vehicle.Registration,
			Make = vehicle.VehicleModel?.Manufacturer?.Name ?? "(missing)",
			Model = vehicle.VehicleModel?.Name ?? "(missing)",
			Color = vehicle.Color,
			Year = vehicle.Year,
			ListedAt = DateTimeOffset.UtcNow
		};
		await bus.PubSub.PublishAsync(message);

	}



	// PUT api/vehicles/ABC123
	[HttpPut("{id}")]
	public IActionResult Put(string id, [FromBody] VehicleDto dto) {
		var vehicleModel = db.FindModel(dto.ModelCode);
		var vehicle = new Vehicle {
			Registration = dto.Registration,
			Color = dto.Color,
			Year = dto.Year,
			ModelCode = vehicleModel.Code
		};
		db.UpdateVehicle(vehicle);
		return Ok(dto);
	}

	// DELETE api/vehicles/ABC123
	[HttpDelete("{id}")]
	public IActionResult Delete(string id) {
		var vehicle = db.FindVehicle(id);
		if (vehicle == default) return NotFound();
		db.DeleteVehicle(vehicle);
		return NoContent();
	}
}
