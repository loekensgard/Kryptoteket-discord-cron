using Kryptoteket.Sync.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kryptoteket.Sync.CosmosDB
{
    public class RegistryContext : DbContext
    {
        public RegistryContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<BetWinner> BetWinners { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<UserBet> UserBets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BetWinner>()
                .HasKey(r => r.id);

            modelBuilder.Entity<BetWinner>()
                .Property(b => b.BetsWon)
                .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
