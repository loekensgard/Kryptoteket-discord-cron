using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.CosmosDB.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly KryptoteketContext _context;
        private readonly DbSet<Bet> _set;
        public BetRepository(KryptoteketContext context)
        {
            _context = context;
            _set = _context.Bets;
        }

        public async Task<ICollection<Bet>> GetBets()
        {
            return await _set.Include(X => X.PlacedBets).ToListAsync();
        }
    }
}
