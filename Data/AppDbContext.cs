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

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<PartidoJugador> PartidoJugadores { get; set; }
        public DbSet<Cancha> Canchas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1) Equipo 1→N Jugador
            modelBuilder.Entity<Equipo>()
                .HasMany(e => e.Jugadores)
                .WithOne(j => j.Equipo)
                .HasForeignKey(j => j.EquipoId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2) Partido N→N Jugador vía PartidoJugador
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
