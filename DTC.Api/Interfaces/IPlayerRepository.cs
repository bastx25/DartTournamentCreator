using DTC.Api.Models;
using System.Numerics;

namespace DTC.Api.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();

        Task<Player?> GetByIdAsync(int id);
    }
}
