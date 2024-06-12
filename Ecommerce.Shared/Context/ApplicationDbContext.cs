

using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.Shared;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Entities.Templates;
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

    public DbSet<Feature> Features { get; set; }
    public DbSet<Cluster> Clusters { get; set; }
    public DbSet<TemplateMaster> TemplateMasters { get; set; }
    public DbSet<TemplateCluster> TemplateClusters { get; set; }
    public DbSet<TemplateClusterFeature> TemplateClusterFeatures { get; set; }
    public DbSet<TemplateCategory> TemplateCategories { get; set; }
    public DbSet<GeneralSize> Sizes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<GeneralColor> Colors { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductFeatureValue> ProductFeatureValues { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<TemplateCategory>()
    .HasIndex(tc => new { tc.TemplateMasterId, tc.CategoryId })
    .IsUnique();
        modelBuilder.Entity<ProductFeatureValue>()
  .HasOne(pfv => pfv.TemplateClusterFeature)
  .WithMany()
  .HasForeignKey(pfv => pfv.TemplateClusterFeatureId)
  .OnDelete(DeleteBehavior.NoAction);

        //base.OnModelCreating(modelBuilder);


        //modelBuilder.Entity<Category>()
        //    .HasMany(c => c.SubCategories)
        //    .WithOne(c => c.ParentCategory)
        //    .HasForeignKey(c => c.ParentCategoryId);
    }
}
