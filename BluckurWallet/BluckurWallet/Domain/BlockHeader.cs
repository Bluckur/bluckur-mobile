using Newtonsoft.Json;

namespace BluckurWallet.Domain
{
    [JsonObject]
    public class BlockHeader
    {
        [JsonProperty]
        public long BlockNumber { get; set; }

        [JsonProperty]
        public string Hash { get; set; }

        [JsonProperty]
        public string PreviousHash { get; set; }

        [JsonProperty]
        public float Reward { get; set; }

        [JsonProperty]
        public long TimeStamp { get; set; }

        [JsonProperty]
        public string Validator { get; set; }

        [JsonProperty]
        public string Version { get; set; }
    }
}
