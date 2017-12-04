using System;
using System.Collections.Generic;
using System.Linq;
using Shuvaev.IDP.Entities;

namespace Shuvaev.IDP.Contextes
{
    public static class UserContextExtensions
    {
		public static void EnsureSeedDataForContext(this UserContext ctx)
	    {
		    if (Queryable.Any<User>(ctx.Users))
		    {
			    return;
		    }

		    // init users
		    var users = new List<User>
		    {
			    new User
			    {
				    SubjectId = new Guid("d860efca-22d9-47fd-8249-791ba61b07c7"),
				    Username = "Frank",
				    Password = "password",
				    IsActive = true,
				    Claims = {
					    new UserClaim {ClaimType = "role", ClaimValue = "FreeUser"},
					    new UserClaim {ClaimType = "given_name", ClaimValue = "Frank"},
					    new UserClaim {ClaimType = "family_name", ClaimValue = "Underwood"},
					    new UserClaim {ClaimType = "address", ClaimValue = "Main Road 1"},
					    new UserClaim {ClaimType = "subscriptionlevel", ClaimValue = "FreeUser"},
					    new UserClaim {ClaimType = "country", ClaimValue = "nl"}
				    }
			    },
			    new User
			    {
				    SubjectId = new Guid("b7539694-97e7-4dfe-84da-b4256e1ff5c7"),
				    Username = "Claire",
				    Password = "password",
				    IsActive = true,
				    Claims = {
					    
					    new UserClaim {ClaimType = "role", ClaimValue = "PayingUser"},
						new UserClaim {ClaimType = "given_name", ClaimValue = "Claire"},
						new UserClaim {ClaimType = "family_name", ClaimValue = "Underwood"},
						new UserClaim {ClaimType = "address", ClaimValue = "Big Street 2"},
						new UserClaim {ClaimType = "subscriptionlevel", ClaimValue = "PayingUser"},
						new UserClaim {ClaimType = "country", ClaimValue = "be"}

					}
			    }
		    };


			ctx.Users.AddRange(users);
		    ctx.SaveChanges();
	    }

	}
}
