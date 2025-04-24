namespace Ktt.Workflows.Core.Services;

public class ConsoleNotificationService : INotificationService
{
    public void Send(string message)
    {
        Console.WriteLine($"[NOTIFY] {message}");
    }
}
