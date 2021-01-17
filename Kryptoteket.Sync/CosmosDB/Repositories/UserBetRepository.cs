using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.CosmosDB.Repositories
{
    public class UserBetRepository : IUserBetRepository
    {
        private readonly KryptoteketContext _context;
        private readonly DbSet<BetUser> _set;
        public UserBetRepository(KryptoteketContext context)
        {
            _context = context;
            _set = _context.BetUsers;
        }

        public async Task AddUser(BetUser betUser)
        {
            _set.Add(betUser);
            await _context.SaveChangesAsync();
        }

        public async Task<BetUser> GetUserBet(ulong id)
        {
            return await _set.Include(x => x.PlacedBets).Include(x => x.Placements).FirstOrDefaultAsync(x => x.BetUserId == id);
        }

        public async Task UpdateUser(BetUser user)
        {
            var entity = await _set.FindAsync(user.BetUserId);
            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
