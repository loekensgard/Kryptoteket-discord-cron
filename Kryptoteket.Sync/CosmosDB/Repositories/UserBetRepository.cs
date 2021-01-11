using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.CosmosDB.Repositories
{
    public class UserBetRepository : IUserBetRepository
    {
        private readonly RegistryContext _context;
        private readonly DbSet<UserBet> _set;
        public UserBetRepository(RegistryContext context)
        {
            _context = context;
            _set = _context.UserBets;
        }

        public async Task<List<UserBet>> GetUserBets(string id)
        {
            var list = _set.Where(r => r.BetId == id);

            return await list.ToListAsync();
        }
    }
}
