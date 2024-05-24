

using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Shared.Context;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<ModelYear> ModelYears { get; set; }
    public DbSet<Brand> Brands { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);


        //modelBuilder.Entity<Category>()
        //    .HasMany(c => c.SubCategories)
        //    .WithOne(c => c.ParentCategory)
        //    .HasForeignKey(c => c.ParentCategoryId);
    }
}
