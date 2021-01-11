using Kryptoteket.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Interfaces
{
    public interface IBetWinnersRepository
    {
        Task AddBetWinner(BetWinner betWinner);
        Task<BetWinner> GetBetWinner(string id);
        Task UpdateBetWinner(BetWinner betWinner);
    }
}
