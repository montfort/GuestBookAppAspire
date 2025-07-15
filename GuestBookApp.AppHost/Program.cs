

var builder = DistributedApplication.CreateBuilder(args);

// A�ade un servidor PostgreSQL y una base de datos llamada "guestbookdb"
var postgres = builder.AddPostgres("postgres");
var guestBookDb = postgres.AddDatabase("guestbookdb");


// Registra el proyecto de migraci�n y le a�ade una referencia a la base de datos
// y espera a que la base de datos est� lista
var migrations = builder.AddProject<Projects.GuestBookApp_MigrationService>("migrations")
   .WithReference(guestBookDb)
   .WaitFor(guestBookDb); // Espera a que el recurso de la DB est� listo


// Registra el proyecto de la API y le a�ade una referencia a la base de datos
var apiService = builder.AddProject<Projects.GuestBookApp_ApiService>("apiservice")
    .WithReference(guestBookDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);




// Registra el proyecto de la interfaz de usuario y le a�ade una referencia a la API
builder.AddProject<Projects.GuestBookApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
