using Microsoft.EntityFrameworkCore;
using Shuvaev.IDP.Entities;

namespace Shuvaev.IDP.Contextes
{
    public sealed class UserContext : DbContext
    {
	    public UserContext(DbContextOptions<UserContext> options) : base((DbContextOptions) options)
	    {
		    RelationalDatabaseFacadeExtensions.Migrate(Database);
	    }

		public DbSet<User> Users { get; set; }
    }
}
