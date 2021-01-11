using System.Text.Json.Serialization;

namespace Kryptoteket.Sync.Models
{
    public class Price
    {
        [JsonPropertyName("bitcoin")]
        public Bitcoin Bitcoin { get; set; }
    }

    public partial class Bitcoin
    {
        [JsonPropertyName("usd")]
        public int Usd { get; set; }
    }
}
