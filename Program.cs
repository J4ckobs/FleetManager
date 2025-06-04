using FleetManager.Models;
using FleetManager.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<DriverService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
