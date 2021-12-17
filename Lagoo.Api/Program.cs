using Lagoo.Api;
using Lagoo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStartup<Startup>();

// builder.Logging.ClearProviders();
//
// builder.Logging.AddConsole();

var app = builder.Build();

await AppDbInitializer.InitializeAsync(app.Services);

app.Run();