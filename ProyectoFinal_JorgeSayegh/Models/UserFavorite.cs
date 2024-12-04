using System;
namespace ProyectoFinal_JorgeSayegh.Models
{
    public class UserFavorite
    {
        public Guid FavoriteId { get; set; } = Guid.NewGuid();

        // Relación: Usuario que ha marcado el widget como favorito
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Relación: Widget marcado como favorito
        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; }

        // Posición del widget en la lista de favoritos
        public int Position { get; set; }
    }
}
