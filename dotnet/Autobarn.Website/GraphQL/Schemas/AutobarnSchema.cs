using Autobarn.Data;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autobarn.Website.GraphQL.Queries;

namespace Autobarn.Website.GraphQL.Schemas {
	public class AutobarnSchema : Schema {
		public AutobarnSchema(IAutobarnDatabase db)
			=> Query = new VehicleQuery(db);
	}
}
