using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using dotenv.net;

namespace alposim.Data;

public class LocalDbContextFactory : IDesignTimeDbContextFactory<LocalDbContext>
{
    public LocalDbContext CreateDbContext(string[] args)
    {
        DotEnv.Load();
        var connectionString = $"Host={Environment.GetEnvironmentVariable("LOCAL_DB_HOST")};Port={Environment.GetEnvironmentVariable("LOCAL_DB_PORT")};Database={Environment.GetEnvironmentVariable("LOCAL_DB_NAME")};Username={Environment.GetEnvironmentVariable("LOCAL_DB_USER")};Password={Environment.GetEnvironmentVariable("LOCAL_DB_PASSWORD")}";

        var options = new DbContextOptionsBuilder<LocalDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new LocalDbContext(options);
    }
}