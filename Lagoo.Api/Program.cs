using Lagoo.Api;
using Lagoo.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

Startup.Configure(app);

var dbInitializer = new AppDbInitializer(app);
await dbInitializer.InitializeAsync();

app.Run();