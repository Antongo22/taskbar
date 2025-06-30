using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace taskbar;

/// <summary>
/// Менеджер панели задач: прозрачность, отображение, автоскрытие и управление проводником.
/// </summary>
internal static class TaskbarManager
{
    private const string TASKBAR_WINDOW_CLASS = "Shell_TrayWnd";
    private const string REG_PATH = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StuckRects3";
    private const string REG_VALUE = "Settings";

    /// <summary>
    /// Включает или отключает автоскрытие панели задач.
    /// </summary>
    /// <param name="enable">true — включить автоскрытие, false — отключить.</param>
    /// <param name="restartExplorer">
    /// true — после изменения настройки сразу перезапустить проводник (explorer.exe),
    /// false — не перезапускать (рекомендуется вызывать RestartExplorer вручную).
    /// </param>
    public static void SetAutoHide(bool enable, bool restartExplorer = true)
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(REG_PATH, writable: true);
            if (key == null)
            {
                Console.WriteLine("❌ Не удалось открыть ключ реестра.");
                return;
            }

            byte[] settings = (byte[])key.GetValue(REG_VALUE)!;

            if (enable)
                settings[8] |= 0x01;  // Установить бит автоскрытия
            else
                settings[8] &= unchecked((byte)~0x01); // Сбросить бит автоскрытия

            key.SetValue(REG_VALUE, settings, RegistryValueKind.Binary);

            if (restartExplorer)
            {
                RestartExplorer();
                Console.WriteLine(enable ? "✅ Автоскрытие включено." : "✅ Автоскрытие отключено.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при изменении автоскрытия: {ex.Message}");
        }
    }

    /// <summary>
    /// Перезапускает процесс Проводника Windows (explorer.exe),
    /// чтобы применить изменения в настройках панели задач.
    /// </summary>
    public static void RestartExplorer()
    {
        try
        {
            foreach (var process in Process.GetProcessesByName("explorer"))
                process.Kill();

            Process.Start("explorer.exe");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при перезапуске Проводника: {ex.Message}");
        }
    }

    /// <summary>
    /// Устанавливает прозрачность панели задач.
    /// </summary>
    /// <param name="alpha">Значение альфа-прозрачности от 0 (полностью прозрачная) до 255 (непрозрачная).</param>
    public static void SetTransparency(byte alpha)
    {
        var handle = GetTaskbarHandle();
        if (handle == IntPtr.Zero)
        {
            Console.WriteLine("❌ Панель задач не найдена.");
            return;
        }

        uint style = GetWindowLong(handle, WinApiConstants.GWL_EXSTYLE);
        SetWindowLong(handle, WinApiConstants.GWL_EXSTYLE, style | WinApiConstants.WS_EX_LAYERED);
        SetLayeredWindowAttributes(handle, 0, alpha, WinApiConstants.LWA_ALPHA);

        Console.WriteLine($"✅ Прозрачность панели задач установлена: {alpha}/255.");
    }

    /// <summary>
    /// Сбрасывает прозрачность панели задач к полностью непрозрачному виду.
    /// </summary>
    public static void ResetTransparency()
    {
        var handle = GetTaskbarHandle();
        if (handle == IntPtr.Zero)
        {
            Console.WriteLine("❌ Панель задач не найдена.");
            return;
        }

        uint style = GetWindowLong(handle, WinApiConstants.GWL_EXSTYLE);
        SetWindowLong(handle, WinApiConstants.GWL_EXSTYLE, style & ~WinApiConstants.WS_EX_LAYERED);
        SetLayeredWindowAttributes(handle, 0, WinApiConstants.FULLY_OPAQUE, WinApiConstants.LWA_ALPHA);

        Console.WriteLine("✅ Прозрачность панели задач сброшена.");
    }

    /// <summary>
    /// Скрывает панель задач (полностью убирает с экрана).
    /// </summary>
    public static void HideTaskbar()
    {
        ShowWindow(GetTaskbarHandle(), WinApiConstants.SW_HIDE);
        Console.WriteLine("✅ Панель задач скрыта.");
    }

    /// <summary>
    /// Показывает панель задач, если она была скрыта.
    /// </summary>
    public static void ShowTaskbar()
    {
        ShowWindow(GetTaskbarHandle(), WinApiConstants.SW_SHOW);
        Console.WriteLine("✅ Панель задач показана.");
    }

    public static void ApplyConfig(Config config)
    {
        if (config.TaskbarVisible)
            ShowTaskbar();
        else
            HideTaskbar();

        SetTransparency(config.Transparency);

        // При изменении автоскрытия не перезапускаем explorer, оставим это на пользователя
        SetAutoHide(config.AutoHide, restartExplorer: false);

        Console.WriteLine("✅ Конфигурация применена (перезапуск Explorer.exe для автоскрытия может потребоваться).");
    }


    


    private static IntPtr GetTaskbarHandle() =>
        FindWindow(TASKBAR_WINDOW_CLASS, null);

    // WinAPI импорты
    [DllImport("user32.dll")] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
