using System;
using System.Collections.Generic;
using System.Text;

namespace Kryptoteket.Sync.Models
{
    public class FinishedBetPlacement
    {
        public int Id { get; set; }
        public int BetId { get; set; }
        public int Place { get; set; }
        public ulong BetUserId { get; set; }
    }
}
