using AutoMapper;
using IdentityServer4;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shuvaev.IDP.Controllers.UserRegistration;
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

	        var config = _configuration.Get<AppOptions>();

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

	        services.AddAuthentication() //CookieAuthenticationDefaults.AuthenticationScheme
										 //.AddCookie(o => o.LoginPath = new PathString("/login"))
				.AddFacebook("FacebookScheme", "Facebook", o =>
		        {
			        o.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					o.AppId = config.Facebook.AppId;
			        o.AppSecret = config.Facebook.AppSecret;
		        });

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

	        AutoMapper.Mapper.Initialize(cfg =>
	        {
		        cfg.CreateMap<RegisterUserViewModel, User>()
					.ForMember(d => d.Username, opt => opt.MapFrom(s => s.UserName));
			});
			

	        userContext.EnsureSeedDataForContext();


			app.UseIdentityServer();
	        
			app.UseStaticFiles();
	        app.UseMvcWithDefaultRoute();

        }
    }
}
