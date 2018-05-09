using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluckurWallet.Domain
{
    [JsonObject]
    public class Transaction
    {
        [JsonProperty]
        public float Amount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Sender { get; set; }

        [JsonProperty]
        public string Recipient { get; set; }

        // TODO: DateTime
        [JsonProperty]
        public long TimeStamp { get; set; }

        [JsonProperty]
        public string Type { get; set; }
    }
}
