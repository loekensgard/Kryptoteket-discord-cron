using Kryptoteket.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Interfaces
{
    public interface IBetWinnersRepository
    {
        Task AddBetWinner(FinishedBetPlacement betWinner);
        Task<FinishedBetPlacement> GetBetWinner(int id);
        Task<List<FinishedBetPlacement>> GetBetWins(ulong userId);
        Task UpdateBetWinner(FinishedBetPlacement betWinner);
    }
}
