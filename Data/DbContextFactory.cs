using Microsoft.EntityFrameworkCore;

namespace alposim.Data;

public class DbContextFactory
{
    private readonly string _localConnectiongString;
    private readonly string _cloudConnectionString;

    public DbContextFactory(string local, string cloud)
    {
        _localConnectiongString = local;
        _cloudConnectionString = cloud;
    }

    public AppDbContext CreateLocal()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_localConnectiongString)
            .Options;
        
        return new AppDbContext(options);
    }

    public AppDbContext CreateCloud()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_cloudConnectionString)
            .Options;
        
        return new AppDbContext(options);
    }
}