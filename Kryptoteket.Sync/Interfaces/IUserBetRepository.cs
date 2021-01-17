using Kryptoteket.Sync.Models;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Interfaces
{
    public interface IUserBetRepository
    {
        Task<BetUser> GetUserBet(ulong id);
        Task AddUser(BetUser betUser);
        Task UpdateUser(BetUser user);
    }
}
