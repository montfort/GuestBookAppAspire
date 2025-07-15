using GuestBookApp.MigrationService;
using Microsoft.EntityFrameworkCore; // Necesario para AddDbContext
using GuestBookApp.Core.Data; // Aseg�rate de que este namespace apunte a tu DbContext

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults(); // Configuraci�n por defecto de Aspire

// Configura el DbContext para usar PostgreSQL con la cadena de conexi�n de Aspire
// "guestbookdb" es el nombre del recurso de base de datos definido en AppHost
builder.AddNpgsqlDbContext<GuestBookDbContext>("guestbookdb");


builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
