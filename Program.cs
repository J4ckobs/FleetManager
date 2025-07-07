using FleetManager.Models;
using FleetManager.Services;
using Microsoft.EntityFrameworkCore;
using Route = FleetManager.Models.Route;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(), // Bieżący katalog
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<FleetContext>(options =>
{
    options.UseSqlServer("Server=DESKTOP-13HITVH;Database=FleetDb;Trusted_Connection=True;TrustServerCertificate=True;");
});

/*
builder.Services.AddScoped<FileDataService<Vehicle>>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new FileDataService<Vehicle>("vehicles", env);
});
builder.Services.AddScoped<FileDataService<Driver>>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new FileDataService<Driver>("drivers", env);
});

builder.Services.AddScoped<FileDataService<Route>>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new FileDataService<Route>("routes", env);
});*/

builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<RouteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
