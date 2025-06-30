using System;
using System.IO;
using System.Text.Json;

namespace taskbar
{
    /// <summary>
    /// Делегат для событий изменения конфигурации.
    /// </summary>
    public delegate void ConfigChangedEventHandler(Config sender);

    /// <summary>
    /// Класс конфигурации настроек панели задач.
    /// </summary>
    public class Config
    {
        private const string ConfigFileName = "taskbar_config.json";

        // Событие вызывается при изменении настроек
        public event ConfigChangedEventHandler? OnConfigChanged;

        // Параметры конфигурации
        public byte Transparency { get; set; } = WinApiConstants.SEMI_TRANSPARENT;
        public bool AutoHide { get; set; } = false;
        public bool TaskbarVisible { get; set; } = true;

        /// <summary>
        /// Загружает конфигурацию из файла.
        /// Если файл не найден, создаёт конфиг с дефолтными значениями.
        /// </summary>
        /// <returns>Загруженный или созданный конфиг</returns>
        public static Config Load()
        {
            if (!File.Exists(ConfigFileName))
            {
                var defaultConfig = new Config();
                defaultConfig.Save();
                return defaultConfig;
            }

            string json = File.ReadAllText(ConfigFileName);
            return JsonSerializer.Deserialize<Config>(json) ?? new Config();
        }

        /// <summary>
        /// Сохраняет текущую конфигурацию в файл.
        /// </summary>
        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFileName, json);
        }

        /// <summary>
        /// Обновляет текущие параметры и вызывает событие изменения конфигурации.
        /// </summary>
        /// <param name="transparency">Прозрачность панели задач</param>
        /// <param name="autoHide">Автоскрытие панели задач</param>
        /// <param name="taskbarVisible">Видимость панели задач</param>
        public void Update(byte transparency, bool autoHide, bool taskbarVisible)
        {
            Transparency = transparency;
            AutoHide = autoHide;
            TaskbarVisible = taskbarVisible;

            OnConfigChanged?.Invoke(this);
        }
    }
}
