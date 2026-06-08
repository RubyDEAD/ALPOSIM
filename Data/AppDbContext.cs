using Microsoft.EntityFrameworkCore;
using alposim.Models;
namespace alposim.Data
{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    //tables
    public DbSet<Product> Products {get; set;}
    public DbSet<Sale> Sales {get; set;}
    public DbSet<SaleItem> SaleItems {get; set;}
    public DbSet<User> Users {get; set;}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<SaleItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
            .HasOne<Sale>()
            .WithMany(si => si.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
}