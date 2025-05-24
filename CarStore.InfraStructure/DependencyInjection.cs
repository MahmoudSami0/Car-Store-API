using CarStore.Application.Common.Interrfaces;
using CarStore.Application.Interfaces;
using CarStore.InfraStructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CarStore.InfraStructure.Authentication;
using CarStore.InfraStructure.Mail;
using CarStore.InfraStructure.Repositorries;
using CarStore.InfraStructure.Services;
using Microsoft.Extensions.DependencyInjection;
using CarStore.Application.Services;
using CarStore.Applcation.Services;

namespace CarStore.InfraStructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraStructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<CarStoreDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("constr")));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionName));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMailService, MailService>();
        services.AddHttpClient<IAIService, AIService>();


        return services;
    }
}