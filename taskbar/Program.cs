using System;

namespace taskbar;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Taskbar Manager";
        Console.ForegroundColor = ConsoleColor.Cyan;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Панель управления Taskbar ===");
            Console.WriteLine("1. Включить прозрачность");
            Console.WriteLine("2. Отключить прозрачность");
            Console.WriteLine("3. Скрыть панель задач");
            Console.WriteLine("4. Показать панель задач");
            Console.WriteLine("5. Свернуть панель задач");
            Console.WriteLine("6. Развернуть панель задач");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TaskbarManager.SetTransparency(WinApiConstants.SEMI_TRANSPARENT);
                    break;
                case "2":
                    TaskbarManager.ResetTransparency();
                    break;
                case "3":
                    TaskbarManager.HideTaskbar();
                    break;
                case "4":
                    TaskbarManager.ShowTaskbar();
                    break;
                case "5":
                    TaskbarManager.SetAutoHide(true);
                    Pause();
                    break;
                case "6":
                    TaskbarManager.SetAutoHide(false);
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
    static void Pause()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}
