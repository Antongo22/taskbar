using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace taskbar;

/// <summary>
/// Управление состоянием панели задач.
/// </summary>
public static class TaskbarManager
{
    private const string TASKBAR_CLASS = "Shell_TrayWnd";
    private const string REG_PATH = @"Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StuckRects3";
    private const string REG_VALUE = "Settings";

    /// <summary>Применяет конфигурацию к панели задач.</summary>
    public static void ApplyConfig(Config config)
    {
        if (config.TaskbarVisible)
            ShowTaskbar();
        else
            HideTaskbar();

        SetTransparency(config.Transparency);

        // Проверим, нужно ли перезапускать Explorer
        bool restartNeeded = SetAutoHide(config.AutoHide, restartExplorer: false);

        if (restartNeeded)
        {
            RestartExplorer();
            Console.WriteLine("♻ Explorer перезапущен для применения автоскрытия.");
        }
        else
        {
            Console.WriteLine("✅ Конфигурация применена.");
        }
    }


    public static bool SetAutoHide(bool enable, bool restartExplorer)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(REG_PATH, writable: true);
            if (key == null)
            {
                Console.WriteLine("❌ Ошибка доступа к реестру.");
                return false;
            }

            byte[] settings = (byte[])key.GetValue(REG_VALUE)!;

            bool currentState = (settings[8] & 0x01) != 0;
            if (currentState == enable)
            {
                return false; // Уже установлено нужное значение
            }

            if (enable)
                settings[8] |= 0x01;
            else
                settings[8] &= unchecked((byte)~0x01);

            key.SetValue(REG_VALUE, settings, RegistryValueKind.Binary);

            if (restartExplorer)
                RestartExplorer();

            return true; // Изменение было
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при изменении автоскрытия: {ex.Message}");
            return false;
        }
    }


    public static void RestartExplorer()
    {
        foreach (var p in Process.GetProcessesByName("explorer"))
            p.Kill();

        Process.Start("explorer.exe");
    }

    public static void SetTransparency(byte alpha)
    {
        var hWnd = FindWindow(TASKBAR_CLASS, null);
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("❌ Панель задач не найдена.");
            return;
        }

        uint styles = GetWindowLong(hWnd, WinApiConstants.GWL_EXSTYLE);
        SetWindowLong(hWnd, WinApiConstants.GWL_EXSTYLE, styles | WinApiConstants.WS_EX_LAYERED);

        bool result = SetLayeredWindowAttributes(hWnd, 0, alpha, WinApiConstants.LWA_ALPHA);
        if (!result)
        {
            Console.WriteLine("❌ Не удалось применить прозрачность.");
        }
        else
        {
            Console.WriteLine($"✅ Прозрачность установлена: {alpha}/255");
        }
    }


    public static void HideTaskbar() =>
        ShowWindow(FindWindow(TASKBAR_CLASS, null), WinApiConstants.SW_HIDE);

    public static void ShowTaskbar() =>
        ShowWindow(FindWindow(TASKBAR_CLASS, null), WinApiConstants.SW_SHOW);

    [DllImport("user32.dll")] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
