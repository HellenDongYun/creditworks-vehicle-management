using CreditWorks.Infrastructure.Persistence;
using CreditWorks.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 1. Configure database connection
// --------------------------------------------------

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured.");
}

// --------------------------------------------------
// 2. Register application services
// --------------------------------------------------

builder.Services.AddDbContext<CreditWorksDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------
// 3. Build application
// --------------------------------------------------

var app = builder.Build();

// --------------------------------------------------
// 4. Seed initial database data
// --------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CreditWorksDbContext>();

    await DatabaseSeeder.SeedAsync(db);
}

// --------------------------------------------------
// 5. Configure HTTP request pipeline
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Vehicle and Category API endpoints
// will be implemented in the next phase.

app.Run();