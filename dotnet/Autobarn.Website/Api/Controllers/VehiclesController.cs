using System.Collections.Generic;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Autobarn.Website.Api.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;

		public VehiclesController(IAutobarnDatabase db) {
			this.db = db;
		}
		
		// GET: api/vehicles
		[HttpGet]
		public IEnumerable<Vehicle> Get()	
			=> db.ListVehicles();
	}
}
