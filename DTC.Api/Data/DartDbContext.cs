using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace DTC.Api.Data
{
    public class DartDbContext : DbContext
    {
        public DartDbContext(DbContextOptions<DartDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // Player
            // =========================================================

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);

            });


            // =========================================================
            // Tournament
            // =========================================================

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasOne(t => t.Config)
                    .WithOne(c => c.Tournament)
                    .HasForeignKey<TournamentConfig>(c => c.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

            });


            // =========================================================
            // TournamentConfig
            // =========================================================

            modelBuilder.Entity<TournamentConfig>(entity =>
            {
                entity.HasKey(c => c.Id);

                // TournamentId muss bei einer 1:1-Beziehung eindeutig sein
                entity.HasIndex(c => c.TournamentId)
                    .IsUnique();
            });

            // =========================================================
            // TournamentPlayer
            // =========================================================

            modelBuilder.Entity<TournamentPlayer>(entity =>
            {
                entity.HasKey(tp => tp.Id);

                // Tournament -> TournamentPlayers
                entity.HasOne(tp => tp.Tournament)
                    .WithMany(t => t.TournamentPlayers)
                    .HasForeignKey(tp => tp.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Player -> TournamentPlayers
                entity.HasOne(tp => tp.Player)
                    .WithMany(p => p.TournamentPlayers)
                    .HasForeignKey(tp => tp.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ein Player darf nur einmal im gleichen Turnier registriert sein
                entity.HasIndex(tp => new
                {
                    tp.TournamentId,
                    tp.PlayerId
                })
                .IsUnique();
            });


            // =========================================================
            // Location
            // =========================================================

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);
            });


            // =========================================================
            // Board
            // =========================================================

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasOne(b => b.Location)
                    .WithMany(l => l.Boards)
                    .HasForeignKey(b => b.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Board-Nummer muss innerhalb einer Location eindeutig sein
                entity.HasIndex(b => new
                {
                    b.LocationId,
                    b.Number
                })
                .IsUnique();

            });


            // =========================================================
            // Group
            // =========================================================

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.Id);

                // Tournament -> Groups
                entity.HasOne(g => g.Tournament)
                    .WithMany(t => t.Groups)
                    .HasForeignKey(g => g.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Gruppenreihenfolge pro Turnier eindeutig
                entity.HasIndex(g => new
                {
                    g.TournamentId,
                    g.Sequence
                })
                .IsUnique();
            });


            // =========================================================
            // GroupPlayer
            // =========================================================

            modelBuilder.Entity<GroupPlayer>(entity =>
            {
                entity.HasKey(gp => gp.Id);

                // Group -> GroupPlayers
                entity.HasOne(gp => gp.Group)
                    .WithMany(g => g.GroupPlayers)
                    .HasForeignKey(gp => gp.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

                // TournamentPlayer -> GroupPlayers
                entity.HasOne(gp => gp.TournamentPlayer)
                    .WithMany(tp => tp.Groups)
                    .HasForeignKey(gp => gp.TournamentPlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Spieler darf nur einmal in derselben Gruppe sein
                entity.HasIndex(gp => new
                {
                    gp.GroupId,
                    gp.TournamentPlayerId
                })
                .IsUnique();
            });


            // =========================================================
            // Braket
            // =========================================================

            modelBuilder.Entity<Braket>(entity =>
            {
                entity.HasKey(r => r.Id);



                // Tournament -> Brakets
                entity.HasOne(r => r.Tournament)
                    .WithMany(t => t.Brakets)
                    .HasForeignKey(r => r.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Reihenfolge innerhalb eines Turniers eindeutig
                entity.HasIndex(r => new
                {
                    r.TournamentId,
                    r.Sequence
                })
                .IsUnique();
            });


            // =========================================================
            // Match
            // =========================================================

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);

                // Braket -> Matches
                // Jedes Match MUSS zu einer Braket gehören
                entity.HasOne(m => m.Braket)
                    .WithMany(r => r.Matches)
                    .HasForeignKey(m => m.BraketId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Group -> Matches
                // Optional, weil KO-Matches keiner Gruppe angehören
                entity.HasOne(m => m.Group)
                    .WithMany(g => g.Matches)
                    .HasForeignKey(m => m.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Board -> Matches
                // Ein Match kann ohne Board angelegt werden.
                // Wird ein Board entfernt, bleibt das Match erhalten.
                entity.HasOne(m => m.Board)
                    .WithMany(b => b.Matches)
                    .HasForeignKey(m => m.BoardId)
                    .OnDelete(DeleteBehavior.SetNull);


            });


            // =========================================================
            // MatchParticipant
            // =========================================================

            modelBuilder.Entity<MatchParticipant>(entity =>
            {
                entity.HasKey(mp => mp.Id);

                // Match -> Participants
                entity.HasOne(mp => mp.Match)
                    .WithMany(m => m.Participants)
                    .HasForeignKey(mp => mp.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // TournamentPlayer -> MatchParticipants
                entity.HasOne(mp => mp.TournamentPlayer)
                    .WithMany(tp => tp.MatchParticipants)
                    .HasForeignKey(mp => mp.TournamentPlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Derselbe TournamentPlayer darf nicht zweimal
                // im gleichen Match vorkommen
                entity.HasIndex(mp => new
                {
                    mp.MatchId,
                    mp.TournamentPlayerId
                })
                .IsUnique();
            });
        }


        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupPlayer> GroupPlayers { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<MatchParticipant> MatchParticipants { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Braket> Brakets { get; set; } = null!;
        public DbSet<Tournament> Tournaments { get; set; } = null!;
        public DbSet<TournamentConfig> TournamentConfigs { get; set; } = null!;
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; } = null!;
    }
}
