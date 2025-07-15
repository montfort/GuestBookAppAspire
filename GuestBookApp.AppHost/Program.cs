

var builder = DistributedApplication.CreateBuilder(args);

// Añade un servidor PostgreSQL y una base de datos llamada "guestbookdb"
var postgres = builder.AddPostgres("postgres");
var guestBookDb = postgres.AddDatabase("guestbookdb");


// Registra el proyecto de migración y le añade una referencia a la base de datos
// y espera a que la base de datos esté lista
var migrations = builder.AddProject<Projects.GuestBookApp_MigrationService>("migrations")
   .WithReference(guestBookDb)
   .WaitFor(guestBookDb); // Espera a que el recurso de la DB esté listo


// Registra el proyecto de la API y le añade una referencia a la base de datos
var apiService = builder.AddProject<Projects.GuestBookApp_ApiService>("apiservice")
    .WithReference(guestBookDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);




// Registra el proyecto de la interfaz de usuario y le añade una referencia a la API
builder.AddProject<Projects.GuestBookApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
