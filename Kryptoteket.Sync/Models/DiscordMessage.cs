using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Kryptoteket.Sync.Models
{
    public class DiscordMessage
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("embeds")]
        public List<Embeds> Embeds { get; set; }
    }

    public partial class Embeds
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
