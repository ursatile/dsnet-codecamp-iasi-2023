using Autobarn.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Autobarn.Website.GraphQL.GraphTypes;
using Autobarn.Website.GraphQL.Schemas;
using EasyNetQ;
using GraphQL;
using GraphQL.Types;
using Microsoft.OpenApi.Models;
using GraphiQl;

namespace Autobarn.Website;

public class Startup {
	protected virtual string DatabaseMode => Configuration["DatabaseMode"];

	public Startup(IConfiguration configuration) {
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services) {
		services.AddRouting(options => options.LowercaseUrls = true);
		services.AddControllersWithViews()
			.AddNewtonsoftJson(options => options.UseCamelCasing(processDictionaryKeys: true));

		services.AddSwaggerGen(options => {
			options.SwaggerDoc("v1", new OpenApiInfo {
				Version = "v1",
				Title = "Autobarn API",
				Description = "The Autobarn vehicle platform API"
			});
		});
		// explicit opt-in - needs to be placed after AddSwaggerGen().
		services.AddSwaggerGenNewtonsoftSupport();

		services.AddRazorPages().AddRazorRuntimeCompilation();

		services.AddSingleton<IAutobarnDatabase, AutobarnCsvFileDatabase>();
		var amqpConnectionString = Configuration.GetConnectionString("rabbitmq_autobarn");
		var bus = RabbitHutch.CreateBus(amqpConnectionString);
		services.AddSingleton(bus);

		services.AddGraphQL(builder => builder
		   //.AddHttpMiddleware<ISchema>()
		   .AddNewtonsoftJson()
		   .AddSchema<AutobarnSchema>()
		   .AddGraphTypes(typeof(VehicleGraphType).Assembly)
	   );
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
		if (env.IsDevelopment()) {
			app.UseDeveloperExceptionPage();
		} else {
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
		app.UseHttpsRedirection();
		app.UseDefaultFiles();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseAuthorization();

		// Add endpoints for exposing the Swagger JSON document describing our API:
		app.UseSwagger();
		// ...and the SwaggerUI interactive API tooling.
		app.UseSwaggerUI();

		app.UseGraphQL<ISchema>();
		app.UseGraphiQl("/graphiql");

		app.UseEndpoints(endpoints => {
			endpoints.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
		});
	}
}
