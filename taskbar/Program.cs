using System;

namespace taskbar;

internal class Program
{
    private static Config _config = Config.Load();

    static void Main(string[] args)
    {
        Console.Title = "Taskbar Manager";
        Console.ForegroundColor = ConsoleColor.Cyan;

        // Подписка на событие изменения конфигурации
        _config.OnConfigChanged += ConfigChangedHandler;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Панель управления Taskbar ===");
            Console.WriteLine("1. Включить прозрачность");
            Console.WriteLine("2. Отключить прозрачность");
            Console.WriteLine("3. Скрыть панель задач");
            Console.WriteLine("4. Показать панель задач");
            Console.WriteLine("5. Включить автоскрытие панели задач");
            Console.WriteLine("6. Отключить автоскрытие панели задач");
            Console.WriteLine("7. Перезапустить Проводник (Explorer.exe)");
            Console.WriteLine("8. Загрузить конфигурацию из файла");
            Console.WriteLine("9. Сохранить текущие настройки в конфиг");
            Console.WriteLine("10. Применить конфигурацию из файла");
            Console.WriteLine("11. Открыть файл конфигурации в редакторе");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TaskbarManager.SetTransparency(WinApiConstants.SEMI_TRANSPARENT);
                    _config.Transparency = WinApiConstants.SEMI_TRANSPARENT;
                    break;
                case "2":
                    TaskbarManager.ResetTransparency();
                    _config.Transparency = WinApiConstants.FULLY_OPAQUE;
                    break;
                case "3":
                    TaskbarManager.HideTaskbar();
                    _config.TaskbarVisible = false;
                    break;
                case "4":
                    TaskbarManager.ShowTaskbar();
                    _config.TaskbarVisible = true;
                    break;
                case "5":
                    TaskbarManager.SetAutoHide(true, restartExplorer: false);
                    _config.AutoHide = true;
                    Console.WriteLine("Автоскрытие включено (перезапуск explorer нужен для вступления в силу).");
                    break;
                case "6":
                    TaskbarManager.SetAutoHide(false, restartExplorer: false);
                    _config.AutoHide = false;
                    Console.WriteLine("Автоскрытие отключено (перезапуск explorer нужен для вступления в силу).");
                    break;
                case "7":
                    TaskbarManager.RestartExplorer();
                    break;
                case "8":
                    _config = Config.Load();
                    Console.WriteLine("Конфигурация загружена.");
                    break;
                case "9":
                    _config.Save();
                    Console.WriteLine("Конфигурация сохранена.");
                    break;
                case "10":
                    var loadedConfig = Config.Load();
                    TaskbarManager.ApplyConfig(loadedConfig);
                    _config = loadedConfig;
                    break;
                case "11":
                    OpenConfigFile();
                    break;
                case "0":
                    Console.WriteLine("Выход...");
                    return;
                default:
                    Console.WriteLine("Неверный ввод. Попробуйте снова.");
                    break;
            }
            Pause();
        }
    }

    private static void ConfigChangedHandler(Config sender)
    {
        Console.WriteLine("⚡ Конфигурация изменилась!");
        // Можно сюда вставить авто-применение, если надо
    }


    private static void OpenConfigFile()
    {
        try
        {
            string configPath = "taskbar_config.json";
            if (!System.IO.File.Exists(configPath))
            {
                Console.WriteLine("⚠️ Файл конфигурации не найден. Создаю новый с настройками по умолчанию.");
                _config.Save();
            }

            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(configPath)
            {
                UseShellExecute = true // чтобы открывать по ассоциации
            };
            process.Start();
            Console.WriteLine("✅ Файл конфигурации открыт в программе по умолчанию.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при открытии файла конфигурации: {ex.Message}");
        }
    }

    static void Pause()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey(intercept: true);
    }
}
