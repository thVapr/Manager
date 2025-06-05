using ManagerData.Contexts;
using ManagerData.Management;
using ManagerData.Authentication;
using ManagerData.Management.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerData;

public static class DependencyInjection
{
    public static IServiceCollection AddManagerData(this IServiceCollection services)
    {
        services.AddDbContext<AuthenticationDbContext>();
        services.AddDbContext<MainDbContext>();

        services.AddScoped<IFileRepository, FileRepository>();
        
        services.AddScoped<IAuthenticationRepository,AuthenticationRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskTypeRepository, TaskTypeRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ITaskMessageRepository, TaskMessageRepository>();
        services.AddScoped<IBackgroundTaskRepository, BackgroundTaskRepository>();
        
        return services;
    }
}