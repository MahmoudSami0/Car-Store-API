using CarStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}