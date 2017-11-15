using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shuvaev.IDP.Entities;
using Shuvaev.IDP.Services;

namespace Shuvaev.IDP
{
    public class Startup
    {
		private static IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddOptions();
			services.Configure<AppOptions>(_configuration);

			var connectionString = _configuration.GetConnectionString("idpUserDbConnection");

			//var connectionString = "test";
			services.AddDbContext<UserContext>(options =>
	        {
		        options.UseSqlServer(connectionString);
	        });

			services.AddScoped<IUserRepository, UserRepository>();

			services.AddMvc();

			services.AddIdentityServer()
				.AddDeveloperSigningCredential()
				.AddShuvaevUserStore()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients());
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger, UserContext userContext)
        {
	        var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();
	        configuration.DisableTelemetry = true;
			logger.AddConsole();
	        logger.AddDebug();

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

	        userContext.EnsureSeedDataForContext();


			app.UseIdentityServer();
	        app.UseStaticFiles();
	        app.UseMvcWithDefaultRoute();

        }
    }
}
