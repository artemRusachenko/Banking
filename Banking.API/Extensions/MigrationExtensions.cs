using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CA1031

namespace Banking.API.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                await dbContext.Database.MigrateAsync().ConfigureAwait(false);
                logger.LogInformation("Migration completed successfully");
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "An error occurred during migration");
                throw;
            }

            return app;
        }
    }
}
