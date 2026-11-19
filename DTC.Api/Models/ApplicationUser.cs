using Microsoft.AspNetCore.Identity;

namespace DTC.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Board> Boards { get; set; } = new List<Board>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
