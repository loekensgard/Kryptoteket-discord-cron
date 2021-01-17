using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;

namespace Kryptoteket.Sync.CosmosDB
{
    public class KryptoteketContext : DbContext
    {
        public KryptoteketContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Bet> Bets { get; set; }
        public DbSet<BetUser> BetUsers { get; set; }
        public DbSet<PlacedBet> PlacedBets { get; set; }
        public DbSet<FinishedBetPlacement> FinishedBetPlacements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
