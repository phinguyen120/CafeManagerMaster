using Microsoft.EntityFrameworkCore;
using G11_Coffee.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Cafe> Cafes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserCafe> UserCafes { get; set; }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        // Thiết lập khóa chính cho bảng UserCafe
        modelBuilder.Entity<UserCafe>()
            .HasKey(mc => new { mc.UserId, mc.CafeId });

        modelBuilder.Entity<UserCafe>()
            .HasOne(mc => mc.User)
            .WithMany(m => m.userCafes)
            .HasForeignKey(mc => mc.UserId);

        modelBuilder.Entity<UserCafe>()
            .HasOne(mc => mc.Cafe)
            .WithMany(c => c.UserCafes)
            .HasForeignKey(mc => mc.CafeId);

        modelBuilder.Entity<OrderDetail>()
        .HasOne(od => od.Product)
        .WithMany()
        .HasForeignKey(od => od.ProductId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetail>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Restrict);


        base.OnModelCreating(modelBuilder);

    }
}
