using ManagerData.Constants;
using ManagerData.DataModels;
using ManagerData.Management;
using ManagerLogic.Authentication;
using ManagerData.DataModels.Authentication;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerLogic.Background;

public class MessengerHostService(IServiceProvider serviceProvider, ILogger<MessengerHostService> logger) : BackgroundService
{
    private int _executionCount;
    private TelegramBotClient botClient;
    private IAuthentication authentication;
    private IBackgroundTaskRepository repository;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"[{DateTime.Now}] Сервис организации уведомлений запущен!");
        using var scope = serviceProvider.CreateScope();
        authentication = scope.ServiceProvider.GetRequiredService<IAuthentication>();
        repository = scope.ServiceProvider.GetRequiredService<IBackgroundTaskRepository>();
        
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
        using var cts = new CancellationTokenSource();
        var secretProvider = new SecretProvider();
        var token = secretProvider.GetBotToken();
        
        botClient = new TelegramBotClient(token!, cancellationToken: cts.Token);
        botClient.OnMessage += OnMessage;
        
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await QueueProcessing();
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"[{DateTime.Now}] Сервис организации уведомлений остановлен,\n {ex}");
        }
    }

    private async Task QueueProcessing()
    {
        try
        {
            int count = Interlocked.Increment(ref _executionCount);
            var nearest = await repository.GetAllNearest();
            
            foreach (var task in nearest)
            {
                var user = await authentication.GetUserById(task.MemberId);
                if (user.Id != task.MemberId || string.IsNullOrEmpty(user.ChatId))
                    continue;
                await HandleMessage(task, user);
            }

            logger.LogInformation($"Сервис успешно обработал текущую очередь. Счёт: {count}\n" +
                                  $"\t{nearest.Count()} действий в очереди");
        }
        catch (Exception ex)
        {
            logger.LogError($"[{DateTime.Now}]\n", ex);
        }
    }

    private async Task HandleMessage(BackgroundTask task, UserDataModel user)
    {
        switch (task.Type)
        {
            case (int)BackgroundTaskType.Available:
            {
                var message = $"📋 Задача: {task.Task.Name}\n➡️ Доступна для вас!\n";
                if (task.Part != null)
                    message += $"💼 {task.Part!.Name}\n";
                if (task.Task.Deadline != null && task.Task.Deadline.Value > DateTime.UnixEpoch)
                    message += $"☎️ Срок сдачи: {task.Task.Deadline}";
                await botClient.SendMessage(user.ChatId!, 
                    message
                    );
                await repository.Delete(task.Id);
            } break;
            case (int)BackgroundTaskType.Removed:
            {
                var message = $"❌ Вы удалены с задачи: \n{task.Task.Name}\n";
                if (task.Part != null)
                    message += $"💼 {task.Part!.Name}\n";
                await botClient.SendMessage(user.ChatId!, 
                    message);
                await repository.Delete(task.Id);
            } break;
            case (int)BackgroundTaskType.StatusUpdate:
            {
                var message = $"📋 Задача: {task.Task.Name}\n🔄 {task.Message}\n";
                await botClient.SendMessage(user.ChatId!, 
                    message);
                await repository.Delete(task.Id);
            } break;
            case (int)BackgroundTaskType.Added:
            {
                var message = $"👤 Пользователь {task.Message} добавлен к задаче: \n{task.Task.Name}\n";
                if (task.Part != null)
                    message += $"💼 {task.Part!.Name}\n";
                await botClient.SendMessage(user.ChatId!, 
                    message);
                await repository.Delete(task.Id);
            } break;
        }
    }
    
    private async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is null) return;
        logger.LogInformation($"[{DateTime.Now}] Полученное сообщение {type} '{msg.Text}' in {msg.Chat}");
        if (msg.Text.StartsWith('/') && msg.Text.Contains($"/register"))
        {
            var user = await authentication.GetUserByMessengerId(msg.From!.Username!);
            if (user.MessengerId == msg.From.Username)
            {
                user.ChatId = msg.Chat.Id.ToString();
                await authentication.UpdateUser(user);
            }
            await botClient.SendMessage(msg.Chat, $"{msg.From!.Username} зарегистрирован");
        }
        else
        {
            await botClient.SendMessage(msg.Chat, $"Не получается распознать следующую команду: {msg.Text}");
        }
    }
    
}