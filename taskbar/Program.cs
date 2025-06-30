using System;

namespace taskbar;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Taskbar Manager";
        Console.ForegroundColor = ConsoleColor.Cyan;

        bool autoHideChanged = false;

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
            Console.WriteLine("7. Перезапустить Проводник (Explorer.exe) (Нужно для преминения 5 и 6 команд)");
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
                    TaskbarManager.SetAutoHide(true, restartExplorer: false);
                    autoHideChanged = true;
                    Console.WriteLine("✅ Автоскрытие включено (требуется рестарт проводника).");
                    break;
                case "6":
                    TaskbarManager.SetAutoHide(false, restartExplorer: false);
                    autoHideChanged = true;
                    Console.WriteLine("✅ Автоскрытие отключено (требуется рестарт проводника).");
                    break;
                case "7":
                    TaskbarManager.RestartExplorer();
                    autoHideChanged = false;
                    Console.WriteLine("✅ Проводник перезапущен.");
                    break;
                case "0":
                    if (autoHideChanged)
                    {
                        Console.WriteLine("⚠️ Изменения автоскрытия требуют перезапуска Проводника.");
                        Console.WriteLine("Вы можете сделать это выбрав пункт 7.");
                    }
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
        Console.ReadKey(intercept: true);
    }
}
