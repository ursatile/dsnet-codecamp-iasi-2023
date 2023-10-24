using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Autobarn.Website.Models;
using Microsoft.CodeAnalysis;

namespace Autobarn.Website.Api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;

		public VehiclesController(IAutobarnDatabase db) {
			this.db = db;
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
		public IActionResult Post([FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				VehicleModel = vehicleModel
			};
			db.CreateVehicle(vehicle);
			return Ok(vehicle);
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
}
