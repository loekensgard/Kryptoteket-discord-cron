using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.CosmosDB.Repositories
{
    public class BetWinnersRepository : IBetWinnersRepository
    {
        private readonly RegistryContext _context;
        private readonly DbSet<BetWinner> _set;
        public BetWinnersRepository(RegistryContext context)
        {
            _context = context;
            _set = _context.BetWinners;
        }

        public async Task<BetWinner> GetBetWinner(string id)
        {
            return await _set.FindAsync(id);
        }

        public async Task AddBetWinner(BetWinner betWinner)
        {
            _set.Add(betWinner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBetWinner(BetWinner betWinner)
        {
            var entity = await _set.FindAsync(betWinner.id);
            if (entity != null)
            {
                _set.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

    }
}
