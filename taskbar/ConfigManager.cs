using System;
using System.Diagnostics;
using System.IO;

namespace taskbar;

/// <summary>
/// Утилиты для работы с конфигурацией.
/// </summary>
public static class ConfigManager
{
    private const string FileName = "taskbar_config.json";

    /// <summary>Открывает конфигурационный файл в программе по умолчанию.</summary>
    public static void OpenConfigFile()
    {
        if (!File.Exists(FileName))
        {
            Console.WriteLine("Файл не найден. Создаю по умолчанию...");
            new Config().Save();
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = FileName,
            UseShellExecute = true
        });

        Console.WriteLine("✅ Конфигурация открыта.");
    }

    /// <summary>Сбрасывает конфигурацию на стандартные настройки Windows.</summary>
    public static Config ResetToDefault()
    {
        var config = new Config
        {
            Transparency = WinApiConstants.FULLY_OPAQUE,
            AutoHide = false,
            TaskbarVisible = true
        };

        config.Save();
        TaskbarManager.ApplyConfig(config);

        Console.WriteLine("✅ Конфигурация сброшена на значения по умолчанию Windows.");
        return config;
    }
}
