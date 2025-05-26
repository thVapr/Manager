using Microsoft.AspNetCore.SignalR;

namespace ManagerLogic.Background;

public class HubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("id")?.Value;
    }
}