
using GuestBookApp.Core.Data;
using GuestBookApp.Core.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Configura el DbContext para usar PostgreSQL con la cadena de conexión de Aspire
builder.Services.AddDbContext<GuestBookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("guestbookdb"))); 
// "guestbookdb" es el nombre del recurso de base de datos definido en AppHost

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();



// Endpoint para obtener todas las firmas
app.MapGet("/guestbook", async (GuestBookDbContext db) =>
{
    return await db.GuestBookEntries.ToListAsync();
});

// Endpoint para añadir una nueva firma
app.MapPost("/guestbook", async (GuestBookEntry entry, GuestBookDbContext db) =>
{
    entry.Date = DateTime.UtcNow; // Asegura la fecha actual
    db.GuestBookEntries.Add(entry);
    await db.SaveChangesAsync();
    return Results.Created($"/guestbook/{entry.Id}", entry);
});


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
