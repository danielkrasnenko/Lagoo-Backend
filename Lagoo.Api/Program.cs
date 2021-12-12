using Lagoo.Api;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStartup<Startup>();

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

var app = builder.Build();

app.Run();