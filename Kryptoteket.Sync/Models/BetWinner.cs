using System;
using System.Collections.Generic;
using System.Text;

namespace Kryptoteket.Sync.Models
{
    public class BetWinner
    {
        public string id { get; set; }
        public int Points { get; set; }
        public string Name { get; set; }
        public string[] BetsWon { get; set; }
    }
}
