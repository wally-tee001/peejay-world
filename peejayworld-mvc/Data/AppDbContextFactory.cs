using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using peejayworld_mvc.Data;

namespace peejayworld_mvc.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

        // Allow the password to be supplied at runtime via env var
        // (ConnectionStrings__DefaultPassword / PGPASSWORD / DB_PASSWORD) so it stays out of source control.
        var password = configuration["ConnectionStrings:DefaultPassword"]
            ?? configuration["PGPASSWORD"]
            ?? configuration["DB_PASSWORD"];

        if (!string.IsNullOrWhiteSpace(password) &&
            !connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase))
        {
            connectionString += $"Password={password};";
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}

