using Blazored.LocalStorage;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Dto;
using Ecommerce.Shared.Services;
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
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddTransient<IBrandService, BrandService>();
builder.Services.AddTransient<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<IPopularBrandService, PopularBrandService>();
builder.Services.AddTransient<ISliderService, SliderService>();
builder.Services.AddScoped<IPopularCategoryService,PopularCategoryService>();
builder.Services.AddScoped<ICategoryConfigurationService, CategoryConfigurationService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<CartStateService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")), ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")), ServiceLifetime.Scoped);
builder.Services.Configure<AdyenSettings>(builder.Configuration.GetSection("Adyen"));
builder.Services.AddHttpClient<IAdyenService, AdyenService>();


builder.Services.AddSingleton<CultureStateService>();

builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
StaticServiceProvider.Configure(app.Services);

string[] supportedCultures = ["en-US","nl-NL", "de-CH"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[2])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);


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
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
