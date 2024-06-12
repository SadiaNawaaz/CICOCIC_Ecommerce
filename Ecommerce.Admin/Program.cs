using Ecommerce.Admin.Components;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.Clusters;
using Ecommerce.Shared.Services.Colors;
using Ecommerce.Shared.Services.Features;
using Ecommerce.Shared.Services.ModelYears;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.ProductVariants;
using Ecommerce.Shared.Services.Sizes;
using Ecommerce.Shared.Services.TemplateCategories;
using Ecommerce.Shared.Services.Templates;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IClusterService, ClusterService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IModelYearService, ModelYearService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITemplateCategoryService, TemplateCategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<IProductVariantService,ProductVariantService>();

builder.Services.AddDbContext<ApplicationDbContext>(
 o => o.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    //config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
    //config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();




app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
