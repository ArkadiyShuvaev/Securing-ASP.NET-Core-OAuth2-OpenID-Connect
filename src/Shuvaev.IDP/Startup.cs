using System;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shuvaev.IDP.Controllers.UserRegistration;
using Shuvaev.IDP.Entities;
using Shuvaev.IDP.Services;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.EntityFramework.DbContexts;
using Shuvaev.IDP.Contextes;

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

	        var migrationsAssembly = typeof(Startup).Assembly.GetName().Name;

			var identityServerDataDbConnectionString =
		        _configuration.GetConnectionString(Consts.IdentityServerDataDbConnectionString);
			services.AddIdentityServer()
				.AddSigningCredential(LoadCertificateFromStore())
				.AddShuvaevUserStore()
				.AddConfigurationStore(options => 
					options.ConfigureDbContext = 
						builder => builder
							.UseSqlServer(identityServerDataDbConnectionString, 
								optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly)));

			services.AddAuthentication()
				.AddCookie("idsrv.2FA")
				.AddFacebook(Consts.FacebookAuthenticationSchemeName, Consts.FacebookDisplayName, o =>
		        {
			        o.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					o.AppId = config.Facebook.AppId;
			        o.AppSecret = config.Facebook.AppSecret;
		        });

		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
			ILoggerFactory logger, UserContext userContext, ConfigurationDbContext configurationDbContext)
        {
	        //var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();
	        //configuration.DisableTelemetry = true;
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

	        configurationDbContext.Database.Migrate();
	        configurationDbContext.EnsureSeedDataForContext();

			userContext.Database.Migrate();
	        userContext.EnsureSeedDataForContext();
			

			app.UseIdentityServer();
	        
			app.UseStaticFiles();
	        app.UseMvcWithDefaultRoute();

        }

	    public X509Certificate2 LoadCertificateFromStore()
	    {
		    // In uppercase
			const string thumbPrint = "8D06B0F693B679CF9F5F0115C6533E33A3F44A12"; 

		    using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
		    {
			    store.Open(OpenFlags.ReadOnly);

			    var certCollection = store.Certificates.Find(
					X509FindType.FindByThumbprint, thumbPrint, true);
			    if (certCollection.Count == 0)
			    {
				    throw new Exception("Tag specified certificate was not found.");
			    }

			    return certCollection[0];
		    }
	    }
    }
}
