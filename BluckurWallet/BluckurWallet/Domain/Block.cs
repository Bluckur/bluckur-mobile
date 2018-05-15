using Newtonsoft.Json;
using System.Collections.Generic;

namespace BluckurWallet.Domain
{
    [JsonObject]
    public class Block
    {
        [JsonProperty]
        public BlockHeader Header { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Transaction> Transactions { get; set; }

        public Block()
        {
            Transactions = new List<Transaction>();
        }
    }
}
