

using System;

namespace ProyectoFinal_JorgeSayegh.Models
{
    public class WidgetSetting
    {
        public Guid SettingId { get; set; } = Guid.NewGuid();
        public string SettingKey { get; set; } // Clave de configuración (e.g., tamaño, color)
        public string Value { get; set; }

        // Relación: Configuración relacionada con un widget
        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; }

        // Relación: Configuración personalizada para un usuario (opcional)
        public Guid? UserId { get; set; }
        public User User { get; set; }
    }
}
