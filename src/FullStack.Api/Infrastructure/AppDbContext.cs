using FullStack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Infrastructure;

/// <summary>
/// Application database context using EF Core InMemory provider.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Gets or sets the product categories.</summary>
    public DbSet<ProductCategory> Categories => Set<ProductCategory>();

    /// <summary>Gets or sets the products.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Slug).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var categories = new List<ProductCategory>
        {
            new(1, "Electronics", "electronics"),
            new(2, "Clothing", "clothing"),
            new(3, "Books", "books"),
            new(4, "Home & Garden", "home-garden"),
            new(5, "Sports", "sports"),
            new(6, "Toys", "toys"),
            new(7, "Food & Beverages", "food-beverages"),
            new(8, "Health & Beauty", "health-beauty"),
            new(9, "Automotive", "automotive"),
            new(10, "Office Supplies", "office-supplies")
        };

        modelBuilder.Entity<ProductCategory>().HasData(categories);

        var products = new List<object>
        {
            new { Id = 1, Name = "Wireless Mouse", Price = 29.99m, Description = "Ergonomic wireless mouse with USB receiver", CategoryId = 1, CreatedAt = DateTimeOffset.Parse("2024-01-15T10:00:00Z"), IsActive = true },
            new { Id = 2, Name = "Mechanical Keyboard", Price = 89.99m, Description = "RGB mechanical keyboard with Cherry MX switches", CategoryId = 1, CreatedAt = DateTimeOffset.Parse("2024-01-16T10:00:00Z"), IsActive = true },
            new { Id = 3, Name = "USB-C Hub", Price = 45.00m, Description = "7-in-1 USB-C hub with HDMI and ethernet", CategoryId = 1, CreatedAt = DateTimeOffset.Parse("2024-01-17T10:00:00Z"), IsActive = true },
            new { Id = 4, Name = "Cotton T-Shirt", Price = 19.99m, Description = "Premium cotton crew neck t-shirt", CategoryId = 2, CreatedAt = DateTimeOffset.Parse("2024-01-18T10:00:00Z"), IsActive = true },
            new { Id = 5, Name = "Denim Jeans", Price = 59.99m, Description = "Classic fit denim jeans", CategoryId = 2, CreatedAt = DateTimeOffset.Parse("2024-01-19T10:00:00Z"), IsActive = true },
            new { Id = 6, Name = "C# in Depth", Price = 39.99m, Description = "Fourth edition covering C# 7 and 8", CategoryId = 3, CreatedAt = DateTimeOffset.Parse("2024-02-01T10:00:00Z"), IsActive = true },
            new { Id = 7, Name = "Clean Architecture", Price = 34.99m, Description = "A craftsman's guide to software structure", CategoryId = 3, CreatedAt = DateTimeOffset.Parse("2024-02-02T10:00:00Z"), IsActive = true },
            new { Id = 8, Name = "Garden Hose", Price = 24.99m, Description = "50ft expandable garden hose", CategoryId = 4, CreatedAt = DateTimeOffset.Parse("2024-02-03T10:00:00Z"), IsActive = true },
            new { Id = 9, Name = "Plant Pot Set", Price = 15.99m, Description = "Set of 3 ceramic plant pots", CategoryId = 4, CreatedAt = DateTimeOffset.Parse("2024-02-04T10:00:00Z"), IsActive = true },
            new { Id = 10, Name = "Yoga Mat", Price = 29.99m, Description = "Non-slip exercise yoga mat", CategoryId = 5, CreatedAt = DateTimeOffset.Parse("2024-02-05T10:00:00Z"), IsActive = true },
            new { Id = 11, Name = "Resistance Bands", Price = 12.99m, Description = "Set of 5 resistance bands", CategoryId = 5, CreatedAt = DateTimeOffset.Parse("2024-02-06T10:00:00Z"), IsActive = true },
            new { Id = 12, Name = "Building Blocks", Price = 34.99m, Description = "500-piece creative building block set", CategoryId = 6, CreatedAt = DateTimeOffset.Parse("2024-02-07T10:00:00Z"), IsActive = true },
            new { Id = 13, Name = "Board Game Collection", Price = 49.99m, Description = "Classic family board games bundle", CategoryId = 6, CreatedAt = DateTimeOffset.Parse("2024-02-08T10:00:00Z"), IsActive = true },
            new { Id = 14, Name = "Organic Coffee", Price = 14.99m, Description = "1kg bag of organic arabica coffee", CategoryId = 7, CreatedAt = DateTimeOffset.Parse("2024-02-09T10:00:00Z"), IsActive = true },
            new { Id = 15, Name = "Green Tea Set", Price = 22.99m, Description = "Premium green tea collection", CategoryId = 7, CreatedAt = DateTimeOffset.Parse("2024-02-10T10:00:00Z"), IsActive = true },
            new { Id = 16, Name = "Vitamin D Supplements", Price = 9.99m, Description = "365-day supply vitamin D3", CategoryId = 8, CreatedAt = DateTimeOffset.Parse("2024-02-11T10:00:00Z"), IsActive = true },
            new { Id = 17, Name = "Face Moisturizer", Price = 18.99m, Description = "Daily hydrating face cream", CategoryId = 8, CreatedAt = DateTimeOffset.Parse("2024-02-12T10:00:00Z"), IsActive = true },
            new { Id = 18, Name = "Car Phone Mount", Price = 16.99m, Description = "Universal magnetic car phone holder", CategoryId = 9, CreatedAt = DateTimeOffset.Parse("2024-02-13T10:00:00Z"), IsActive = true },
            new { Id = 19, Name = "Desk Organizer", Price = 27.99m, Description = "Bamboo desk organizer with drawers", CategoryId = 10, CreatedAt = DateTimeOffset.Parse("2024-02-14T10:00:00Z"), IsActive = true },
            new { Id = 20, Name = "Sticky Notes Pack", Price = 8.99m, Description = "Multicolor sticky notes 12-pack", CategoryId = 10, CreatedAt = DateTimeOffset.Parse("2024-02-15T10:00:00Z"), IsActive = true }
        };

        modelBuilder.Entity<Product>().HasData(products);
    }
}
