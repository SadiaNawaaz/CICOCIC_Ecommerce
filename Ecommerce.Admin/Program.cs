using Blazored.SessionStorage;
using Ecommerce.Admin.Authentication;
using Ecommerce.Admin.Components;
using Ecommerce.Admin.Components.Pages;
using Ecommerce.Admin.Services;
using Ecommerce.Mailer;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Services;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Catalogs;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.CategoryConfigurations;
using Ecommerce.Shared.Services.CategoryFeatures;
using Ecommerce.Shared.Services.Clusters;
using Ecommerce.Shared.Services.Colors;
using Ecommerce.Shared.Services.Configurations;
using Ecommerce.Shared.Services.Features;
using Ecommerce.Shared.Services.Languages;
using Ecommerce.Shared.Services.ModelYears;
using Ecommerce.Shared.Services.PopularBrands;
using Ecommerce.Shared.Services.PopularCategories;
using Ecommerce.Shared.Services.Products;
using Ecommerce.Shared.Services.ProductVariants;
using Ecommerce.Shared.Services.RolePermissions;
using Ecommerce.Shared.Services.Roles;
using Ecommerce.Shared.Services.Sizes;
using Ecommerce.Shared.Services.TemplateCategories;
using Ecommerce.Shared.Services.Templates;
using Ecommerce.Shared.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IClusterService, ClusterService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddTransient<IPopularBrandService, PopularBrandService>();
builder.Services.AddTransient<IModelYearService, ModelYearService>();
builder.Services.AddTransient<IFeatureService, FeatureService>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<ITemplateCategoryService, TemplateCategoryService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ISizeService, SizeService>();
builder.Services.AddTransient<IColorService, ColorService>();
builder.Services.AddTransient<IProductVariantService,ProductVariantService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddSingleton<IMailerServices, MailerServices>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ImageResizeService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddTransient<ICatalogService, CatalogService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IPopularCategoryService, PopularCategoryService>();
builder.Services.AddScoped<CsvExportService>();
builder.Services.AddScoped<ICategoryConfigurationService, CategoryConfigurationService>();
builder.Services.AddScoped<IDataDownloads, DataDownloads>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ICategoryFeatureService, CategoryFeatureService>();

//builder.Services.AddDbContext<ApplicationDbContext>(
// o => o.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")), ServiceLifetime.Scoped);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")), ServiceLifetime.Scoped);




//builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = 104857600; });
builder.Services.AddSignalR(opt => opt.MaximumReceiveMessageSize = 102400000);
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

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddHttpContextAccessor();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<SidebarStateService>();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/login"; // Path to the login page
//        options.AccessDeniedPath = "/accessdenied"; // Path to the access denied page
//    });
builder.Services.AddAuthentication("Auth")
           .AddCookie("Auth", options =>
           {
               options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
               options.SlidingExpiration = true;
               options.LoginPath = "/login";
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

app.UseAuthentication();
app.UseAuthorization();


app.UseRouting();
app.UseAntiforgery();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller}/{action}");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
