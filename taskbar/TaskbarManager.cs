using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace taskbar;

/// <summary>
/// Менеджер панели задач: прозрачность, отображение, автоскрытие.
/// </summary>
internal static class TaskbarManager
{
    private const string TASKBAR_WINDOW_CLASS = "Shell_TrayWnd";
    private const string REG_PATH = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StuckRects3";
    private const string REG_VALUE = "Settings";

    public static void SetAutoHide(bool enable)
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

            RestartExplorer();
            Console.WriteLine(enable ? "✅ Автоскрытие включено." : "✅ Автоскрытие отключено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при изменении автоскрытия: {ex.Message}");
        }
    }

    private static void RestartExplorer()
    {
        foreach (var process in Process.GetProcessesByName("explorer"))
            process.Kill();

        Process.Start("explorer.exe");
    }


    public static void SetTransparency(byte alpha)
    {
        var handle = GetTaskbarHandle();
        if (handle == IntPtr.Zero) return;

        uint style = GetWindowLong(handle, WinApiConstants.GWL_EXSTYLE);
        SetWindowLong(handle, WinApiConstants.GWL_EXSTYLE, style | WinApiConstants.WS_EX_LAYERED);
        SetLayeredWindowAttributes(handle, 0, alpha, WinApiConstants.LWA_ALPHA);
    }

    public static void ResetTransparency()
    {
        var handle = GetTaskbarHandle();
        if (handle == IntPtr.Zero) return;

        uint style = GetWindowLong(handle, WinApiConstants.GWL_EXSTYLE);
        SetWindowLong(handle, WinApiConstants.GWL_EXSTYLE, style & ~WinApiConstants.WS_EX_LAYERED);
        SetLayeredWindowAttributes(handle, 0, WinApiConstants.FULLY_OPAQUE, WinApiConstants.LWA_ALPHA);
    }

    public static void HideTaskbar()
    {
        ShowWindow(GetTaskbarHandle(), WinApiConstants.SW_HIDE);
    }

    public static void ShowTaskbar()
    {
        ShowWindow(GetTaskbarHandle(), WinApiConstants.SW_SHOW);
    }

    private static IntPtr GetTaskbarHandle() =>
        FindWindow(TASKBAR_WINDOW_CLASS, null);

    // WinAPI
    [DllImport("user32.dll")] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
