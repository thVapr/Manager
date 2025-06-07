namespace ManagerData.Constants;
using Microsoft.Extensions.Configuration;

public class SecretProvider
{
    private IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddUserSecrets<SecretProvider>()
        .Build();

    public string? GetAuthConnection()
    {
        return Configuration["ManagerAuth"];
    }
    
    public string? GetMainConnection()
    {
        return Configuration["ManagerData"];
    }

    public string? GetStorageAccessKey()
    {
        return Configuration["StorageAccessKey"];
    }

    public string? GetStorageSecretKey()
    {
        return Configuration["StorageSecretKey"];
    }
    
    public string? GetBotToken()
    {
        return Configuration["TelegramBotToken"];
    }

    public string? GetTokenSecretKey()
    {
        return Configuration["TokenSecretKey"];
    }
}