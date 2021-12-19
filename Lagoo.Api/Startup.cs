using Lagoo.Api.Common.Extensions;
using Lagoo.BusinessLogic;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Helpers;
using Lagoo.Domain.Enums;
using Lagoo.Infrastructure;
using Lagoo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;

namespace Lagoo.Api;

public static class Startup
{
    private const string SpecificationInternalUiRout = "/api";
    
    private const string SpecificationRoute = "/api/specification.json";

    private const string HealthCheckRoute = "/health";

    private const string ResourcesPath = "Resources";

    private const string CorsPolicyName = "EnableCORS";

    // Add services to the container.
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalization(options => options.ResourcesPath = ResourcesPath);
        
        services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        // services.AddEndpointsApiExplorer();

        services.AddInfrastructure(configuration);

        services.AddBusinessLogic(configuration);
        
        services.AddAuth(configuration);

        services.AddConfiguredCors();
        
        services.AddAutoGeneratedApi();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    // Configure the HTTP request pipeline.
    public static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseCors(CorsPolicyName);

        var supportedCultures = CultureHelper.GetSupportedCulturesInfo();
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(Culture.En.GetEnumDescription()),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });
        
        app.UseCustomExceptionHandler();
        
        app.UseHealthChecks(HealthCheckRoute);

        app.UseOpenApi(settings => settings.Path = SpecificationRoute);

        app.UseReDoc(settings =>
        {
            settings.Path = SpecificationInternalUiRout;
            settings.DocumentPath = SpecificationRoute;
        });
        
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}