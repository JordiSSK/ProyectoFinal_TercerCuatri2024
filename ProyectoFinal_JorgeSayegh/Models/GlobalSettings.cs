namespace ProyectoFinal_JorgeSayegh.Models;

public class GlobalSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int DefaultRefreshInterval { get; set; } // Intervalo global de actualización
    public string DefaultApiKey { get; set; } // API Key predeterminada
    public string DefaultWidgetSize { get; set; } // Tamaño predeterminado de widgets
}
