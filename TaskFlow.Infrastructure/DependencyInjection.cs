using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Security;
namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        var jwtSettings = new JwtSettings
        {
            Secret = configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret ayarı bulunamadı."),
            Issuer = configuration["Jwt:Issuer"] ?? "TaskFlow",
            Audience = configuration["Jwt:Audience"] ?? "TaskFlowUsers",
            ExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60")
        };

        services.AddSingleton(jwtSettings);
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}