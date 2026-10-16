using CreditWorks.Api.Exceptions;
using CreditWorks.Application.Interfaces;
using CreditWorks.Application.Services;
using CreditWorks.Infrastructure.Persistence;
using CreditWorks.Infrastructure.Persistence.Repositories;
using CreditWorks.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Database connection
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured.");
}

builder.Services.AddDbContext<CreditWorksDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryManagementService, CategoryManagementService>();

// 3. Application services
builder.Services.AddScoped<IVehicleService, VehicleService>();

// 4. Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 6. Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CreditWorksDbContext>();

    await DatabaseSeeder.SeedAsync(db);
}

// 7. Middleware
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Allows WebApplicationFactory in integration tests
public partial class Program { }