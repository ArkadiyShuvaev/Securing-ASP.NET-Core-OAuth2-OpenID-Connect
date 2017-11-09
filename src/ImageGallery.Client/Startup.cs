using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.Client
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

	        services.AddAuthorization(options =>
	        {
				options.AddPolicy(
					"CanOrderFrame", policyBuilder =>
				{
					policyBuilder.RequireAuthenticatedUser();
					policyBuilder.RequireClaim("country", "be");
					policyBuilder.RequireClaim("subscriptionlevel", "PayingUser");
				});
	        });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
	        loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }


	        const string authenticationScheme = "Cookies";
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationScheme = authenticationScheme,
				AccessDeniedPath = new PathString("/Authorization/AccessDenied")
			});

			// Resetting the claim mapping dictionary 
			// ensures the original claim types are kept
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


			app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
	        {
				AuthenticationScheme = "oidc",
				Authority = "https://localhost:44393/",
				RequireHttpsMetadata = true,
				ClientId = "imagegalleryclient",
				Scope = { "openid", "profile", "address", "roles", "imagegalleryapi", "country", "subscriptionlevel" },
				ResponseType = "code id_token",
				//CallbackPath = "...",
				//SignedOutCallbackPath = new PathString(""),
				SignInScheme = authenticationScheme,
				SaveTokens = true,
				ClientSecret = "secret",
				GetClaimsFromUserInfoEndpoint = true,
				Events = new OpenIdConnectEvents
				{
					OnTokenValidated = tokenValidatedContext =>
					{
						var identity = tokenValidatedContext.Ticket.Principal.Identity as ClaimsIdentity;
						if (identity != null)
						{
							var subjectClaim = identity.Claims.FirstOrDefault(c => c.Type == "sub");
							var newClaimsIdentity = new ClaimsIdentity(tokenValidatedContext.Ticket.AuthenticationScheme, 
								"given_name", "role");

							newClaimsIdentity.AddClaim(subjectClaim);

							tokenValidatedContext.Ticket = new AuthenticationTicket(
								new ClaimsPrincipal(newClaimsIdentity), 
								tokenValidatedContext.Ticket.Properties,
								tokenValidatedContext.Ticket.AuthenticationScheme);
						}
						return Task.FromResult(0);
					},

					OnUserInformationReceived = context =>
					{
						context.User.Remove("address"); //remove existing claim from the user information context
						return Task.FromResult(0);
					}
				}
			});

            app.UseStaticFiles();

    
			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }         
    }
}
