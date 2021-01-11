using Kryptoteket.Sync.Configurations;
using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Services
{
    public class DiscordWebhookService : IDiscordWebhookService
    {
        private readonly HttpClient _httpClient;
        private readonly DiscordConfiguration _options;

        public DiscordWebhookService(HttpClient httpClient, IOptions<DiscordConfiguration> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<bool> PostWinners(DiscordMessage body)
        {
            using (var requets = new HttpRequestMessage(HttpMethod.Post, $"api/webhooks/{_options.Id}/{_options.Webhook}"))
            {
                requets.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                using (var response = await _httpClient.SendAsync(requets, HttpCompletionOption.ResponseHeadersRead))
                {
                    return response.IsSuccessStatusCode;
                }
            }
        }

    }
}
