using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories;

namespace FUNewsManagementSystem.Architecture;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app, ILogger logger)
    {
        logger.LogInformation("Applying migrations...");
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<Prn232Lab1DbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Migrations applied successfully!");
    }
}
