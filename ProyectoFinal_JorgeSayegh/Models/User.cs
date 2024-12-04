
using System;
using System.Collections.Generic;
namespace ProyectoFinal_JorgeSayegh.Models
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // Relación: Un usuario puede tener múltiples widgets
        public ICollection<Widget> Widgets { get; set; } = new List<Widget>();

        // Relación: Un usuario puede tener múltiples configuraciones de widgets
        public ICollection<WidgetSetting> WidgetSettings { get; set; } = new List<WidgetSetting>();

        // Relación: Un usuario puede tener múltiples favoritos
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    }
}
