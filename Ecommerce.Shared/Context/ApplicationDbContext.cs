

using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Brands;
using Ecommerce.Shared.Entities.Catalogs;
using Ecommerce.Shared.Entities.CategoryConfigurations;
using Ecommerce.Shared.Entities.CategoryFeatures;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Colors;
using Ecommerce.Shared.Entities.Configurations;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.ModelYears;
using Ecommerce.Shared.Entities.Orders;
using Ecommerce.Shared.Entities.Permissions;
using Ecommerce.Shared.Entities.PopularBrands;
using Ecommerce.Shared.Entities.PopularCategories;
using Ecommerce.Shared.Entities.Products;
using Ecommerce.Shared.Entities.ProductVariants;
using Ecommerce.Shared.Entities.Roles;
using Ecommerce.Shared.Entities.Shared;
using Ecommerce.Shared.Entities.Sizes;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Entities.TrendingProducts;
using Ecommerce.Shared.Services.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using static Ecommerce.Shared.Services.ProductVariants.ProductVariantService;

namespace Ecommerce.Shared.Context;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<PopularCategory> PopularCategories { get; set; }


    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<ModelYear> ModelYears { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<PopularBrand> PopularBrands { get; set; }
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
    public DbSet<ProductVariantImages> productVariantImages { get; set; }
    public DbSet<VariantObjectMedia> VariantObjectMedias { get; set; }
    
    public DbSet<ProductVariantFeatureValue> ProductVariantFeatureValues { get; set; }
    public DbSet<TrendingProduct> TrendingProducts { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<ProductImages> ProductImages { get; set; }
    public DbSet<CatalogMedia> CatalogMedias { get; set; }
    public DbSet<ProductMedia> ProductMedias { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<ProductVariantMedia> ProductVariantMedias { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<CategoryConfiguration> CategoryConfigurations { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<CategoryFeature> CategoryFeatures { get; set; }

    

    public DbSet<OrderItem> OrderItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Catalog>()
       .HasOne(c => c.Category)
       .WithMany()
       .HasForeignKey(c => c.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Catalog>()
    .HasIndex(p => new { p.Name, p.Id,p.Code })
    .IsUnique();



        modelBuilder.Entity<Catalog>()
       .HasOne(c => c.Category)
       .WithMany()
       .HasForeignKey(c => c.CategoryId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
       .HasOne(c => c.Brand)
       .WithMany()
       .HasForeignKey(c => c.BrandId)
       .OnDelete(DeleteBehavior.Restrict);




       modelBuilder.Entity<Feature>()
      .HasOne(f => f.Cluster)
      .WithMany(c => c.Features)
      .HasForeignKey(f => f.ClusterId)
      .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TemplateCategory>()
       .HasIndex(tc => new { tc.TemplateMasterId, tc.CategoryId })
       .IsUnique();
        modelBuilder.Entity<ProductFeatureValue>()
       .HasOne(pfv => pfv.TemplateClusterFeature)
       .WithMany()
       .HasForeignKey(pfv => pfv.TemplateClusterFeatureId)
       .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<ProductVariantFeatureValue>()
       .HasOne(pvfv => pvfv.TemplateClusterFeature)
       .WithMany()
       .HasForeignKey(pvfv => pvfv.TemplateClusterFeatureId)
       .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<RawClusterFeatureDto>().HasNoKey();
        //base.OnModelCreating(modelBuilder);


        //modelBuilder.Entity<Category>()
        //    .HasMany(c => c.SubCategories)
        //    .WithOne(c => c.ParentCategory)
        //    .HasForeignKey(c => c.ParentCategoryId);

        modelBuilder.Entity<UserRole>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId);

    }


}

