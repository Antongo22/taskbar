using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace taskbar;

internal static class WinApiConstants
{
    // Индекс для получения/установки расширенного стиля окна
    public const int GWL_EXSTYLE = -20;

    // Стиль окна: поддержка слоя для прозрачности
    public const uint WS_EX_LAYERED = 0x80000;

    // Флаг для использования альфа-канала прозрачности
    public const uint LWA_ALPHA = 0x2;

    // Видимость окна
    public const int SW_HIDE = 0;             // Полностью скрыть
    public const int SW_SHOW = 5;             // Полностью показать
    public const int SW_MINIMIZE = 6;         // Свернуть (аналог иконки)
    public const int SW_RESTORE = 9;          // Восстановить из свёрнутого состояния

    // Уровни прозрачности
    public const byte FULLY_OPAQUE = 255;     // Непрозрачный
    public const byte SEMI_TRANSPARENT = 150; // Полупрозрачный
}
