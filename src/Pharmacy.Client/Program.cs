namespace Pharmacy.Client;

public delegate void ApiLogHandler(string message);

class Program
{
    static event ApiLogHandler? OnLog;

    static async Task Main()
    {
        // Подписка
        ApiLogHandler consoleLog = msg => Console.WriteLine($"[LOG]: {msg}");
        ApiLogHandler fileLog = msg => File.AppendAllText("api.log", $"{DateTime.Now}: {msg}\n");

        OnLog += consoleLog;
        OnLog += fileLog;

        OnLog?.Invoke("Запрос списка лекарств...");
        // Тут логика HttpClient...

        // Отписка (требование лабы)
        OnLog -= fileLog;
        OnLog?.Invoke("Файловое логирование отключено.");
    }
}