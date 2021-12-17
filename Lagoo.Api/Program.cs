using Lagoo.Api;
using Lagoo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

// builder.Logging.ClearProviders();
//
// builder.Logging.AddConsole();

var app = builder.Build();

Startup.Configure(app);

await AppDbInitializer.InitializeAsync(app.Services);

app.Run();