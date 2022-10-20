using System.Security.Claims;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Hubs.Events;
using Lagoo.Infrastructure.AppOptions.Services;
using Lagoo.Infrastructure.Services.JwtAuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Lagoo.Api.Common.Extensions;

/// <summary>
///   Authentication and Authorization extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class AuthServiceCollectionExtensions
{
    public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtAuthConfig = configuration.GetSection(JwtAuthOptions.JwtAuth).Get<JwtAuthOptions>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtAuthConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAuthConfig.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtAuthService.GetSymmetricSecurityKey(jwtAuthConfig.Secret),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Path.StartsWithSegments(EventsHub.Route) &&
                            context.Request.Query.TryGetValue("access_token", out var accessToken) &&
                            !accessToken.IsNullOrEmpty())
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();
        });
    }
}