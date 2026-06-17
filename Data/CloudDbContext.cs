using Microsoft.EntityFrameworkCore;

namespace alposim.Data;

public class CloudDbContext : AppDbContext
{
    public CloudDbContext(DbContextOptions<CloudDbContext> options) : base(options) { }
    
}