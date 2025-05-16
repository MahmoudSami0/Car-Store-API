using CarStore.Applcation.Profiles;
using CarStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}