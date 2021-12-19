using Lagoo.Api;
using Lagoo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.Configure(app);

var dbInitializer = new AppDbInitializer(app);
await dbInitializer.InitializeAsync();

app.Run();