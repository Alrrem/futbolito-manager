using FutbolitoManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserStory> UserStories { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<PartidoJugador> PartidoJugadores { get; set; }
        public DbSet<Cancha> Canchas { get; set; }
        public DbSet<DailyLog> DailyLogs { get; set; }     // ← DailyLogs
        public DbSet<SprintReview> SprintReviews { get; set; }
        public DbSet<SprintRetrospective> SprintRetrospectives { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SprintReview>()
    .HasOne(r => r.Sprint)
    .WithMany()
    .HasForeignKey(r => r.SprintId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SprintRetrospective>()
                .HasOne(r => r.Sprint)
                .WithMany()
                .HasForeignKey(r => r.SprintId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación 1:N Sprint → UserStory
            modelBuilder.Entity<Sprint>()
                .HasMany(s => s.UserStories)
                .WithOne(us => us.Sprint)
                .HasForeignKey(us => us.SprintId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación 1:N Sprint → DailyLog
            modelBuilder.Entity<Sprint>()
                .HasMany(s => s.DailyLogs)
                .WithOne(dl => dl.Sprint)
                .HasForeignKey(dl => dl.SprintId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1) Equipo 1→N Jugador (cascade)
            modelBuilder.Entity<Equipo>()
                .HasMany(e => e.Jugadores)
                .WithOne(j => j.Equipo)
                .HasForeignKey(j => j.EquipoId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2) Partido N→N Jugador vía PartidoJugador (cascade sobre ambos FK)
            modelBuilder.Entity<PartidoJugador>()
                .HasKey(pj => pj.Id);

            modelBuilder.Entity<PartidoJugador>()
                .HasOne(pj => pj.Partido)
                .WithMany(p => p.PartidoJugadores)
                .HasForeignKey(pj => pj.PartidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PartidoJugador>()
                .HasOne(pj => pj.Jugador)
                .WithMany(j => j.PartidoJugadores)
                .HasForeignKey(pj => pj.JugadorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
