// Program.cs
using System;

namespace taskbar;

internal class Program
{
    private static Config _config = Config.LoadOrCreate();

    static void Main(string[] args)
    {
        Console.Title = "Taskbar Manager";
        Console.ForegroundColor = ConsoleColor.Cyan;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Панель управления Taskbar ===\n");
            Console.WriteLine("1. Открыть файл конфигурации в редакторе");
            Console.WriteLine("2. Сбросить конфигурацию к настройкам по умолчанию");
            Console.WriteLine("3. Применить конфигурацию");
            Console.WriteLine("4. Информация о конфигурации");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ConfigManager.OpenConfigFile();
                    break;
                case "2":
                    _config = ConfigManager.ResetToDefault();
                    break;
                case "3":
                    _config = Config.LoadOrCreate();
                    TaskbarManager.ApplyConfig(_config);
                    break;
                case "4":
                    _config.PrintInfo();
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

    /// <summary>
    /// Пауза до нажатия клавиши пользователем.
    /// </summary>
    private static void Pause()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey(true);
    }
}
