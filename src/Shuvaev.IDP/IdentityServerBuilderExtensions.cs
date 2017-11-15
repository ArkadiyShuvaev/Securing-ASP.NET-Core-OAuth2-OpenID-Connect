using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shuvaev.IDP.Services;

namespace Shuvaev.IDP
{
    public static class IdentityServerBuilderExtensions
    {
		public static IIdentityServerBuilder AddShuvaevUserStore(this IIdentityServerBuilder builder)
		{

			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.AddProfileService<UserProfileService>();

			return builder;
		}
    }
}
