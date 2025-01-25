using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PollForge.Application.Abstractions.Authentication;
using PollForge.Infrastructure.Authentication;
using PollForge.Infrastructure.Database;
using PollForge.Infrastructure.Time;
using PollForge.SharedKernel;
using PollForge.Application.Abstractions.Data;

namespace PollForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = configuration["Authentication:Audience"]!;
        options.MetadataAddress = configuration["Authentication:DiscoveryUrl"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration["Authentication:Issuer"]!
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Request.Cookies.TryGetValue("ID_TOKEN", out var idToken);

                if (!string.IsNullOrEmpty(idToken))
                {
                    context.Token = idToken;
                }
                
                return Task.CompletedTask;
            }
        };
    });

        services.AddDbContext<PollForgeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddHttpContextAccessor();
        services.AddSingleton<IUserContext, UserContext>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IKeycloakApi, KeycloakApi>();

        services.AddScoped<IPollForgeDbContext>(sp => sp.GetRequiredService<PollForgeDbContext>());

        return services;
    }
}