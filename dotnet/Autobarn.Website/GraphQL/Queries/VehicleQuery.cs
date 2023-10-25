using System;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Website.GraphQL.Queries;

public sealed class VehicleQuery : ObjectGraphType {
	private readonly IAutobarnDatabase db;

	public VehicleQuery(IAutobarnDatabase db) {
		this.db = db;

		Field<ListGraphType<VehicleGraphType>>("Vehicles")
			.Description("Return all vehicles")
			.Resolve(GetAllVehicles);

		Field<VehicleGraphType>("Vehicle")
			.Description("Get a single car")
			.Arguments(MakeNonNullStringArgument("registration", "The registration of the vehicle you want"))
			.Resolve(GetVehicle);

		Field<ListGraphType<VehicleGraphType>>("VehiclesByColor")
			.Description("Query to retrieve all Vehicles matching the specified color")
			.Arguments(MakeNonNullStringArgument("color", "The name of a color, eg 'blue', 'grey'"))
			.Resolve(GetVehiclesByColor);
	}

	private QueryArgument MakeNonNullStringArgument(string name, string description) {
		return new QueryArgument<NonNullGraphType<StringGraphType>> {
			Name = name, Description = description
		};
	}

	private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context)
		=> db.ListVehicles();

	private Vehicle GetVehicle(IResolveFieldContext<object> context) {
		var registration = context.GetArgument<string>("registration");
		return db.FindVehicle(registration);
	}

	private IEnumerable<Vehicle> GetVehiclesByColor(IResolveFieldContext<object> context) {
		var color = context.GetArgument<string>("color");
		var vehicles = db.ListVehicles()
			.Where(v => v.Color.Contains(color, StringComparison.InvariantCultureIgnoreCase));
		return vehicles;
	}
}
