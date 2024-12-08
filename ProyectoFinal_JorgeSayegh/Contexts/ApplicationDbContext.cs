using Microsoft.EntityFrameworkCore;
using ProyectoFinal_JorgeSayegh.Models;

namespace ProyectoFinal_JorgeSayegh.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<WidgetSetting> WidgetSettings { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<GlobalSettings> GlobalSettings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación uno a muchos: Usuario -> Widgets
            modelBuilder.Entity<Widget>()
                .HasOne(w => w.User)
                .WithMany(u => u.Widgets)
                .HasForeignKey(w => w.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction); // No cascada para evitar conflictos

            // Relación uno a muchos: Widget -> WidgetSettings
            modelBuilder.Entity<WidgetSetting>()
                .HasKey(ws => ws.SettingId); // Define la clave primaria
            modelBuilder.Entity<WidgetSetting>()
                .HasOne(ws => ws.Widget)
                .WithMany(w => w.Settings)
                .HasForeignKey(ws => ws.WidgetId)
                .OnDelete(DeleteBehavior.NoAction); // No cascada para evitar conflictos

            // Relación opcional uno a muchos: Usuario -> WidgetSettings
            modelBuilder.Entity<WidgetSetting>()
                .HasOne(ws => ws.User)
                .WithMany(u => u.WidgetSettings)
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Permitir SetNull para relaciones opcionales

            // Relación muchos a muchos simulada: UserFavorite entre Usuarios y Widgets
            modelBuilder.Entity<UserFavorite>()
                .HasKey(uf => uf.FavoriteId); // Define la clave primaria

            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.NoAction); // No cascada para evitar conflictos

            modelBuilder.Entity<UserFavorite>()
                .HasOne(uf => uf.Widget)
                .WithMany(w => w.UserFavorites)
                .HasForeignKey(uf => uf.WidgetId)
                .OnDelete(DeleteBehavior.NoAction); // No cascada para evitar conflictos
        }
    }
}
