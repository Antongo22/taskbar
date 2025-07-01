using System;
using System.IO;
using System.Text.Json;

namespace taskbar;

/// <summary>
/// Конфигурация панели задач.
/// </summary>
public class Config
{
    private const string FileName = "taskbar_config.json";

    /// <summary>Прозрачность панели задач (0–255).</summary>
    public byte Transparency { get; set; } = WinApiConstants.SEMI_TRANSPARENT;

    /// <summary>Автоскрытие панели задач.</summary>
    public bool AutoHide { get; set; } = false;

    /// <summary>Видимость панели задач.</summary>
    public bool TaskbarVisible { get; set; } = true;

    /// <summary>
    /// Загружает конфигурацию или создаёт дефолтную.
    /// </summary>
    public static Config LoadOrCreate()
    {
        if (!File.Exists(FileName))
        {
            var defaultConfig = new Config();
            defaultConfig.Save();
            return defaultConfig;
        }

        string json = File.ReadAllText(FileName);
        return JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }

    /// <summary>Сохраняет конфигурацию в файл.</summary>
    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }

    /// <summary>Печатает описание конфигурации в консоль.</summary>
    public void PrintInfo()
    {
        Console.WriteLine("\n=== Текущая конфигурация ===");
        Console.WriteLine($"Прозрачность: {Transparency} (0 = прозрачно, 255 = непрозрачно)");
        Console.WriteLine($"Автоскрытие: {(AutoHide ? "включено" : "отключено")}");
        Console.WriteLine($"Видимость панели задач: {(TaskbarVisible ? "показана" : "скрыта")}");
    }
}
