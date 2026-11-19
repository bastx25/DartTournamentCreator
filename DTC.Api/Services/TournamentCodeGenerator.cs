using System.Security.Cryptography;
using DTC.Api.Data;
using DTC.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DTC.Api.Services
{
    public class TournamentCodeGenerator : ITournamentCodeGenerator
    {
        private readonly DartDbContext _context;
        // Unambiguous characters: no 0/O, 1/I, L
        private const string Chars = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
        private const int CodeLength = 6;
        private const string Prefix = "DART-";

        public TournamentCodeGenerator(DartDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueTournamentCodeAsync()
        {
            const int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var randomPart = GenerateRandomString(CodeLength);
                var candidate = $"{Prefix}{randomPart}";

                var exists = await _context.Tournaments.AnyAsync(t => t.Code == candidate);
                if (!exists)
                {
                    return candidate;
                }
            }

            // Fallback for extreme collision case: append extra timestamp entropy
            return $"{Prefix}{GenerateRandomString(CodeLength)}{RandomNumberGenerator.GetInt32(10, 99)}";
        }

        private static string GenerateRandomString(int length)
        {
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                int index = RandomNumberGenerator.GetInt32(Chars.Length);
                buffer[i] = Chars[index];
            }
            return new string(buffer);
        }
    }
}
