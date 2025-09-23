using Carrental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Configuration;
using Google.Api;
using Google.Apis.Auth.OAuth2;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services; 
using Carrental.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//Google Auth
builder.Services.AddAuthentication()
               .AddGoogle(options =>
               {
                   options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                   options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
               });
/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});
*/

builder.Services.AddTransient<EmailConfigurationService>();
// In ConfigureServices method of Startup.cs
builder.Services.AddTransient<Carrental.Services.IEmailSender, EmailSender>(); 
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("AuthMessageSenderOptions"));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<CarContext, CarContext>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICarVersionRepository, CarVersionRepository>();
builder.Services.AddScoped<ICarFilters, CarFilters>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//db
builder.Services.AddDbContext<CarContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));
});


builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));
// Add services to the container.
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<CarContext>().AddDefaultTokenProviders();
//builder.Services.ConfigureApplicationCookie(opt => )

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf+binary";
provider.Mappings[".gltf"] = "model/gltf+json";

// Configure static files for car1
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/car1")),
    RequestPath = "/car1",
    ContentTypeProvider = provider
});

// Configure static files for car2
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/car2")),
    RequestPath = "/car2",
    ContentTypeProvider = provider
});

// Configure static files for car3
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/car3")),
    RequestPath = "/car3",
    ContentTypeProvider = provider
});
// Configure static files for car4
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/car4")),
    RequestPath = "/car4",
    ContentTypeProvider = provider
});


app.UseStaticFiles();




app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
