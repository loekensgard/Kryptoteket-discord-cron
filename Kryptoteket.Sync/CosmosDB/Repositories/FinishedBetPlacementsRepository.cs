using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.CosmosDB.Repositories
{
    public class FinishedBetPlacementsRepository : IBetWinnersRepository
    {
        private readonly KryptoteketContext _context;
        private readonly DbSet<FinishedBetPlacement> _set;
        public FinishedBetPlacementsRepository(KryptoteketContext context)
        {
            _context = context;
            _set = _context.FinishedBetPlacements;
        }

        public async Task<List<FinishedBetPlacement>> GetBetWins(ulong userId)
        {
            return await _set.Where(x => x.BetUserId == userId).ToListAsync();
        }

        public async Task<FinishedBetPlacement> GetBetWinner(int id)
        {
            return await _set.FindAsync(id);
        }

        public async Task AddBetWinner(FinishedBetPlacement betWinner)
        {
            _set.Add(betWinner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBetWinner(FinishedBetPlacement betWinner)
        {
            var entity = await _set.FindAsync(betWinner.Id);
            if (entity != null)
            {
                _set.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

    }
}
