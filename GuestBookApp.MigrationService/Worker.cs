using GuestBookApp.Core.Data; // Asegúrate de que este namespace apunte a tu DbContext
using GuestBookApp.Core.Models; // Asegúrate de que este namespace apunte a tus modelos
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace GuestBookApp.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GuestBookDbContext>();

            // Aplica las migraciones pendientes
            await RunMigrationAsync(dbContext, cancellationToken);

            // Sembrado de datos (si no lo haces en OnModelCreating con HasData)
            // Si ya lo haces en OnModelCreating con HasData, puedes omitir esta llamada
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
        hostApplicationLifetime.StopApplication(); // Detiene el worker una vez completado
    }

    private static async Task RunMigrationAsync(GuestBookDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Ejecuta la migración dentro de una transacción para mayor seguridad
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(GuestBookDbContext dbContext, CancellationToken cancellationToken)
    {
        // Solo si no hay entradas existentes, para evitar duplicados en cada inicio
        if (!await dbContext.GuestBookEntries.AnyAsync(cancellationToken))
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Sembrar datos iniciales (ejemplo, si no usas HasData en OnModelCreating)
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                dbContext.GuestBookEntries.AddRange(
                    new GuestBookEntry { Name = "Alicia Domínguez", Date = DateTime.UtcNow.AddDays(-5), Comment = "¡Qué gran libro de firmas! Me encanta." },
                    new GuestBookEntry { Name = "Roberto Gómez", Date = DateTime.UtcNow.AddDays(-3), Comment = "Excelente iniciativa. ¡Saludos desde la red!" },
                    new GuestBookEntry { Name = "Juan Pérez", Date = DateTime.UtcNow.AddDays(-1), Comment = "He dejado mi huella digital aquí. ¡Gracias!" }
                );
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });
        }
    }
}