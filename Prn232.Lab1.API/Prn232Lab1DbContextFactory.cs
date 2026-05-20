using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Prn232.Lab1.Repositories;

namespace Prn232.Lab1.API
{
    public class Prn232Lab1DbContextFactory : IDesignTimeDbContextFactory<Prn232Lab1DbContext>
    {
        public Prn232Lab1DbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<Prn232Lab1DbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(Prn232Lab1DbContext).Assembly.FullName));

            return new Prn232Lab1DbContext(optionsBuilder.Options);
        }
    }
}
