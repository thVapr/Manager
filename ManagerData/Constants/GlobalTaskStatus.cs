using System.ComponentModel;
using System.Reflection;

namespace ManagerData.Constants;

public enum GlobalTaskStatus
{
    [Description("Новая")] New = 0,
    [Description("В работе")] InProgress,
    [Description("Оценка")] Estimation,
    [Description("Завершенная")] Completed,
    [Description("Отмененная")] Cancelled,
    [Description("Пауза")] Paused
}