using Kryptoteket.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Interfaces
{
    public interface IDiscordWebhookService
    {
        Task<bool> PostWinners(DiscordMessage body);
    }
}
