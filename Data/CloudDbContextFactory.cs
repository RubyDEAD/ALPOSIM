using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using dotenv.net;

namespace alposim.Data;

public class CloudDbContextFactory : IDesignTimeDbContextFactory<CloudDbContext>
{
    public CloudDbContext CreateDbContext(string[] args)
    {
        DotEnv.Load();
        var connectionString = $"Host={Environment.GetEnvironmentVariable("CLOUD_DB_HOST")};Database={Environment.GetEnvironmentVariable("CLOUD_DB_NAME")};Username={Environment.GetEnvironmentVariable("CLOUD_DB_USER")};Password={Environment.GetEnvironmentVariable("CLOUD_DB_PASSWORD")};SSL Mode=Require;Trust Server Certificate=true";

        var options = new DbContextOptionsBuilder<CloudDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CloudDbContext(options);
    }
}