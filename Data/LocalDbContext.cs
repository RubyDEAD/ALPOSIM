using Microsoft.EntityFrameworkCore;

namespace alposim.Data;

public class LocalDbContext : AppDbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }
}