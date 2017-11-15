using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shuvaev.IDP.Entities
{
    public sealed class UserContext : DbContext
    {
	    public UserContext(DbContextOptions<UserContext> options) : base(options)
	    {
		    Database.Migrate();
	    }

		public DbSet<User> Users { get; set; }
    }
}
