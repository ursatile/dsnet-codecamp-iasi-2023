using System.Collections;
using System.Collections.Generic;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Autobarn.Website.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModelsController : ControllerBase {
	private readonly IAutobarnDatabase db;

	public ModelsController(IAutobarnDatabase db) {
		this.db = db;
	}

	//[DisableCors]
	[HttpGet]
	public IEnumerable<Model> Get()
		=> db.ListModels();
}
