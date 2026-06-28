using Microsoft.EntityFrameworkCore;
using alposim.Models;
namespace alposim.Data
{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Product> Products {get; set;}
    public DbSet<Sale> Sales {get; set;}
    public DbSet<SaleItem> SaleItems {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sync> Syncs { get; set; }
    public DbSet<ProductHistory>  ProductHistories { get; set; }
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

            modelBuilder.Entity<Sync>()
                .Property(s => s.Status)
                .HasConversion<string>();
            
            modelBuilder.Entity<Sync>()
                .Property(s => s.Method)
                .HasConversion<string>();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
}