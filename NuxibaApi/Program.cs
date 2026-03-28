using Microsoft.EntityFrameworkCore;
using NuxibaApi.Data;
using NuxibaApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        await DataSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error al sembrar datos iniciales:");
        Console.WriteLine(ex.Message);
    }
}

// OpenAPI solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();