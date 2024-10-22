using Blazored.LocalStorage;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.CategoryConfigurations;
using Ecommerce.Shared.Services.Configurations;
using Ecommerce.Shared.Services.Customers;
using Ecommerce.Shared.Services.PopularBrands;
using Ecommerce.Shared.Services.PopularCategories;
using Ecommerce.Shared.Services.ProductVariants;
using Ecommerce.Web;
using Ecommerce.Web.Components;
using Ecommerce.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddTransient<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<IPopularBrandService, PopularBrandService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IPopularCategoryService,PopularCategoryService>();
builder.Services.AddScoped<ICategoryConfigurationService, CategoryConfigurationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<CartStateService>();
//builder.Services.AddDbContext<ApplicationDbContext>(
// o => o.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")),
    ServiceLifetime.Transient);
builder.Services.AddBlazoredLocalStorage();
var app = builder.Build();
string serverlessBaseURI = builder.Configuration["ApiUrl"];
UrlHelper.Initialize(serverlessBaseURI);
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
