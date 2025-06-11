using FleetManager.Models;
using FleetManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllersWithViews();


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

builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DriverService>();

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
