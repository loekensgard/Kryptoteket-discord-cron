using System;
using System.Collections.Generic;
using System.Text;

namespace Kryptoteket.Sync.Models
{
    public class BetUser
    {
        public ulong BetUserId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public ICollection<FinishedBetPlacement> Placements { get; set; }
        public ICollection<PlacedBet> PlacedBets { get; set; }
    }
}
