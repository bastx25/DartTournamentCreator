using DTC.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTC.Api.Data
{
    public class DartDbContext : DbContext
    {
        public DartDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Board Constraints
            modelBuilder.Entity<Board>()
                .HasIndex(b => new { b.LocationId, b.Number })
                .IsUnique(); // In einer Location darf jede Board-Nummer nur einmal vorkommen

            // Round Constraints
            modelBuilder.Entity<Round>()
                .HasIndex(r => new { r.TournamentId, r.Sequence })
                .IsUnique(); // In einem Turnier ist die Runden-Sequenz eindeutig

            // Delete Behaviors (Verhindert versehentliches Kaskadieren beim Löschen von Stammdaten)
            modelBuilder.Entity<Round>()
                .HasOne(r => r.Location)
                .WithMany(l => l.Rounds)
                .HasForeignKey(r => r.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Board)
                .WithMany(b => b.Matches)
                .HasForeignKey(m => m.BoardId)
                .OnDelete(DeleteBehavior.SetNull); // Wird ein Board gelöscht/deaktiviert, bleibt das Match bestehen
        }

        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<Tournament> Tournaments { get; set; } = null!;
        public DbSet<Round> Rounds { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<MatchParticipant> MatchParticipants { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;



    }
}