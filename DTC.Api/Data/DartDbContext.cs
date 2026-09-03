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

    modelBuilder.Entity<Board>()
        .HasIndex(b => new { b.LocationId, b.Number })
        .IsUnique();

    modelBuilder.Entity<Round>()
        .HasIndex(r => new { r.TournamentId, r.Sequence })
        .IsUnique();

    modelBuilder.Entity<Group>()
        .HasIndex(g => new { g.TournamentId, g.Sequence })
        .IsUnique();

    modelBuilder.Entity<GroupPlayer>()
        .HasIndex(gp => new { gp.GroupId, gp.PlayerId })
        .IsUnique();

    // Round -> Location
    modelBuilder.Entity<Round>()
        .HasOne(r => r.Location)
        .WithMany(l => l.Rounds)
        .HasForeignKey(r => r.LocationId)
        .OnDelete(DeleteBehavior.Restrict);

    // Match -> Round
    // IMPORTANT: Prevents multiple cascade paths in SQL Server.
    modelBuilder.Entity<Match>()
        .HasOne(m => m.Round)
        .WithMany(r => r.Matches)
        .HasForeignKey(m => m.RoundId)
        .OnDelete(DeleteBehavior.Restrict);

    // Match -> Board
    modelBuilder.Entity<Match>()
        .HasOne(m => m.Board)
        .WithMany(b => b.Matches)
        .HasForeignKey(m => m.BoardId)
        .OnDelete(DeleteBehavior.SetNull);

    // Match -> Group
    modelBuilder.Entity<Match>()
        .HasOne(m => m.Group)
        .WithMany(g => g.Matches)
        .HasForeignKey(m => m.GroupId)
        .OnDelete(DeleteBehavior.SetNull);

    // Group -> Tournament
    modelBuilder.Entity<Group>()
        .HasOne(g => g.Tournament)
        .WithMany(t => t.Groups)
        .HasForeignKey(g => g.TournamentId)
        .OnDelete(DeleteBehavior.Cascade);

    // Group -> Board
    modelBuilder.Entity<Group>()
        .HasOne(g => g.Board)
        .WithMany(b => b.Groups)
        .HasForeignKey(g => g.BoardId)
        .OnDelete(DeleteBehavior.Restrict);

    // GroupPlayer -> Group
    modelBuilder.Entity<GroupPlayer>()
        .HasOne(gp => gp.Group)
        .WithMany(g => g.Players)
        .HasForeignKey(gp => gp.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

    // GroupPlayer -> Player
    modelBuilder.Entity<GroupPlayer>()
        .HasOne(gp => gp.Player)
        .WithMany(p => p.Groups)
        .HasForeignKey(gp => gp.PlayerId)
        .OnDelete(DeleteBehavior.Cascade);
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
