using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shuvaev.IDP.Contextes
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserContext>
	{
		public UserContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			var builder = new DbContextOptionsBuilder<UserContext>();
			var connectionString = configuration.GetConnectionString("idpUserDbConnection");
			builder.UseSqlServer(connectionString);
			return new UserContext(builder.Options);
		}
	}
}
