using DTC.Api.Models;
using Microsoft.EntityFrameworkCore;

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

                entity.Property(p => p.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Nickname)
                    .HasMaxLength(100);
            });


            // =========================================================
            // Tournament
            // =========================================================

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Description)
                    .HasMaxLength(2000);
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

                entity.Property(l => l.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(l => l.Address)
                    .HasMaxLength(500);
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

                entity.Property(b => b.Label)
                    .HasMaxLength(100);
            });


            // =========================================================
            // Group
            // =========================================================

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(100);

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
                    .WithMany(g => g.Players)
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
            // Round
            // =========================================================

            modelBuilder.Entity<Round>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .HasMaxLength(100);

                // Tournament -> Rounds
                entity.HasOne(r => r.Tournament)
                    .WithMany(t => t.Rounds)
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

                // Round -> Matches
                // Jedes Match MUSS zu einer Round gehören
                entity.HasOne(m => m.Round)
                    .WithMany(r => r.Matches)
                    .HasForeignKey(m => m.RoundId)
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

                // Zeiten
                entity.Property(m => m.PlannedStart)
                    .IsRequired(false);

                entity.Property(m => m.PlannedEnd)
                    .IsRequired(false);

                entity.Property(m => m.ActualStart)
                    .IsRequired(false);

                entity.Property(m => m.ActualEnd)
                    .IsRequired(false);
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


        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<Tournament> Tournaments { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupPlayer> GroupPlayers { get; set; } = null!;
        public DbSet<Round> Rounds { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<MatchParticipant> MatchParticipants { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
    }
}
