using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_JorgeSayegh.Models
{
    public class Widget
    {
        [Key]
        public Guid WidgetId { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, ErrorMessage = "The Name field cannot exceed 100 characters.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "The Type field is required.")]
        public string Type { get; set; }

        [Url(ErrorMessage = "The Data URL must be a valid URL.")]
        public string DataUrl { get; set; }

        [Range(1, 3600, ErrorMessage = "The Refresh Interval must be between 1 and 3600 seconds.")]
        public int RefreshInterval { get; set; } = 300;

        public bool IsFavorite { get; set; } = false;

        // Relación: Creador del widget
        [Required]
        public Guid CreatedBy { get; set; }
        public User? User { get; set; }

        // Relación: Un widget puede tener múltiples configuraciones
        public ICollection<WidgetSetting> Settings { get; set; } = new List<WidgetSetting>();

        // Relación: Un widget puede ser favorito de múltiples usuarios
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
    }
}