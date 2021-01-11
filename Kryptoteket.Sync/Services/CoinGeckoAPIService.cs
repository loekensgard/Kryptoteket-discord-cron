using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Sync.Services
{
    public class CoinGeckoAPIService : ICoinGeckoAPIService
    {
        private readonly HttpResponseService _httpResponseService;
        private readonly HttpClient _httpClient;

        public CoinGeckoAPIService(HttpResponseService httpResponseService, HttpClient httpClient)
        {
            _httpResponseService = httpResponseService;
            _httpClient = httpClient;
        }
        public async Task<Price> GetBtcPrice()
        {
            using (var requets = new HttpRequestMessage(HttpMethod.Get, "api/v3/simple/price?ids=bitcoin&vs_currencies=usd"))
            {
                using (var response = await _httpClient.SendAsync(requets, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                        return await _httpResponseService.DeserializeJsonFromStream<Price>(response);
                    else
                        return null;
                }
            }
        }
    }
}
