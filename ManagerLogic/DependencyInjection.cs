using System.Text;
using ManagerData;
using ManagerData.Management;
using ManagerData.Management.Implementation;
using ManagerLogic.Authentication;
using ManagerLogic.Background;
using ManagerLogic.Management;
using ManagerLogic.Management.Implementation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ManagerLogic;

public static class DependencyInjection
{
    public static IServiceCollection AddManagerLogic(this IServiceCollection services)
    {
        services.AddManagerData();
        
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider,HubUserIdProvider>();
        services.AddScoped<IAuthentication,Authentication.Authentication>();
        services.AddScoped<IEncrypt,Encrypt>();
        services.AddScoped<IJwtCreator,JwtCreator>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IPartLogic, PartLogic>();
        services.AddScoped<IPathHelper, PathHelper>();
        services.AddScoped<IMemberLogic, MemberLogic>();
        services.AddScoped<ITaskTypeLogic, TaskTypeLogic>();
        services.AddScoped<ITaskLogic, TaskLogic>();
        services.AddScoped<IFileLogic, FileLogic>();
        services.AddScoped<ITaskMessageLogic, TaskMessageLogic>();
        services.AddHostedService<MessengerHostService>();

        return services;
    }
}
