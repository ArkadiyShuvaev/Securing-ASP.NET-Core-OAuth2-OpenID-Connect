using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Secret = IdentityServer4.Models.Secret;

namespace Shuvaev.IDP.Contextes
{
    public static class ConfigurationDbContextExtensions
    {
	    public static void EnsureSeedDataForContext(this ConfigurationDbContext context)
	    {
		    if (!context.Clients.Any())
		    {
			    foreach (var client in Config.GetClients())
			    {
				    context.Clients.Add(client.ToEntity());
			    }
			    context.SaveChanges();
		    }


		    if (!context.ApiResources.Any())
		    {
			    foreach (var apiResource in Config.GetApiResources())
			    {
				    context.ApiResources.Add(apiResource.ToEntity());
			    }
			    context.SaveChanges();
		    }

		    if (!context.IdentityResources.Any())
		    {
			    foreach (var identityResource in Config.GetIdentityResources())
			    {
				    context.IdentityResources.Add(identityResource.ToEntity());
			    }
			    context.SaveChanges();
		    }
			

		}

    }
}
