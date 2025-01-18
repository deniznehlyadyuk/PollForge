using Application.Abstractions.Data;
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
using System.Text;

namespace PollForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddDbContext<PollForgeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddHttpContextAccessor();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IUserContext, UserContext>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IPollForgeDbContext>(sp => sp.GetRequiredService<PollForgeDbContext>());

        return services;
    }
}