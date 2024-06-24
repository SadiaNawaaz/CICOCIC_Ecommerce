using Ecommerce.Shared.Context;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.ProductVariants;
using Ecommerce.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddDbContext<ApplicationDbContext>(
 o => o.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true); 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
