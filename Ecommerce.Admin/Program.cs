using Blazored.SessionStorage;
using Ecommerce.Admin.Authentication;
using Ecommerce.Admin.Components;
using Ecommerce.Admin.Services;
using Ecommerce.Mailer;
using Ecommerce.Shared.Context;
using Ecommerce.Shared.Services.Brands;
using Ecommerce.Shared.Services.Catalogs;
using Ecommerce.Shared.Services.Categories;
using Ecommerce.Shared.Services.Clusters;
using Ecommerce.Shared.Services.Colors;
using Ecommerce.Shared.Services.Configurations;
using Ecommerce.Shared.Services.Features;
using Ecommerce.Shared.Services.ModelYears;
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
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddDbContext<ApplicationDbContext>(
 o => o.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")));
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
