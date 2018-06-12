using System;
using BluckurWallet.ServiceLayer;
using Newtonsoft.Json;

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
        
        /// <summary>
        /// Gets or sets the unix timestamp of this transaction.
        /// </summary>
        [JsonProperty]
        public long TimeStamp { get; set; }

        /// <summary>
        /// Gets the <see cref="TimeStamp"/> as a formatted date time.
        /// </summary>
        [JsonIgnore]
        public DateTime Time
        {
            get
            {
                return TimeStamp.ToDateTime();
            }
        }

        [JsonProperty]
        public string Type { get; set; }
    }
}
