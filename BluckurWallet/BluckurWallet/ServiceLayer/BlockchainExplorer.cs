using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BluckurWallet.ServiceLayer
{
    /// <summary>
    /// A helper class to obtain data from the REST blockchain explorer service.
    /// The class doesn't consume exceptions, but they are wrapped in a <see cref="RestException"/> for easier parsing.
    /// </summary>
    public class BlockchainExplorer
    {
        public static string Address { get; set; } = "http://192.168.135.2:3000/api/";

        private static readonly Dictionary<string, string> endpoints = new Dictionary<string, string>()
        {
            { "blockByHash",                "block/byhash/{0}" },
            { "blockByNumber",              "block/bynumber/{0}" },
            { "nextBlock",                  "block/next/{0}" },
            { "previousBlock",              "block/previous/{0}" },
            { "blocksByDate",               "blocks/date/{0}" },
            { "blocksByPeriod",             "blocks/period/{0}/{1}" },
            { "blocksByWallet",             "blocks/wallet/{0}" },
            { "nextBlocks",                 "blocks/next/{0}" },
            { "transactionsWithWallet",     "transactions/{0}" },
            { "transactionsFromWallet",     "transactions/from/{0}" },
            { "transactionsToWallet",       "transactions/to/{0}" },
            { "transactionsFromToWallet",   "transactions/from/{0}/to/{1}" },
            { "transactionsFromDate",       "transactions/period/{0}" },
            { "transactionsByPeriod",       "transactions/period/{0}/{1}" },
            { "transactionsByDate",         "transactions/date/{0}}" },
            { "transactionsByAmount",       "transactions/amount/{0}" },
            { "transactionsWithinAmount",   "transactions/amount/{0}/{1}" }
        };

        RestConsumer consumer;

        /// <summary>
        /// Instantiates a new helper class for the blockchain explorer.
        /// </summary>
        public BlockchainExplorer()
        {
            consumer = new RestConsumer();

            UpdateAddress();
            Application.Current.PropertyChanged += PropertyChanged;
        }
        
        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Doesn't fire when the properties are changed in the HelpPage!
            if (e.PropertyName == "explorerServer")
            {
                UpdateAddress();
            }
        }

        private void UpdateAddress()
        {
            object explorerServerObj = null;
            if (Application.Current.Properties.TryGetValue("explorerServer", out explorerServerObj))
            {
                Address = explorerServerObj.ToString();
            }
        }

        #region Ednpoint calls

        public async Task<Block> GetBlock(string hash)
        {
            try
            {
                // TODO: Verify hash (hex of length x?)
                string url = string.Format(Address + endpoints["blockByHash"],
                    hash
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));
                JObject jResponse = response.JsonBody;
                return jResponse["block"].ToObject<Block>();
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid hash.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<Block> GetBlock(UInt64 number)
        {
            try
            {
                string url = string.Format(Address + endpoints["blockByNumber"],
                    number
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));
                JObject jResponse = response.JsonBody;
                return jResponse["block"].ToObject<Block>();
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid block number.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<Block> NextBlock(Block block)
        {
            try
            {
                string url = string.Format(Address + endpoints["nextBlock"],
                    block.Header.Hash
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return jResponse["block"].ToObject<Block>();
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid block.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<LinkedList<Block>> NextBlocks(string hash)
        {
            try
            {
                string url = string.Format(Address + endpoints["nextBlocks"],
                    hash
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseBlockChain((JArray)jResponse["blocks"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid block.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<Block> PreviousBlock(Block block)
        {
            try
            {
                string url = string.Format(Address + endpoints["previousBlock"],
                    block.Header.Hash
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return jResponse["block"].ToObject<Block>();
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid block.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        /// <summary>
        /// Gets all blocks in the blockchain added in the given day (based on UTC time).
        /// </summary>
        /// <param name="day">Day.</param>
        /// <returns>Blocks of the day.</returns>
        /// <exception cref="RestException"/>
        public async Task<LinkedList<Block>> GetBlocksAsync(DateTime day)
        {
            try
            {
                day = new DateTime(day.Year, day.Month, day.Day);
                var nextDay = day.AddDays(1).AddSeconds(-1);
                
                string url = string.Format(Address + endpoints["blocksByPeriod"],
                    day.ToUnix(),
                    nextDay.ToUnix()
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseBlockChain((JArray)jResponse["blocks"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        /// <summary>
        /// Get all blocks in the blockchain added between the given days (inclusive).
        /// </summary>
        /// <param name="from">First day.</param>
        /// <param name="to">Last day.</param>
        /// <returns>Blocks between from and to.</returns>
        /// <exception cref="RestException"/>
        public async Task<LinkedList<Block>> GetBlocksAsync(DateTime from, DateTime to)
        {
            try
            {
                long unixFrom = from.ToUnix();
                long unixTo = to.ToUnix();
                string url = string.Format(Address + endpoints["blocksByPeriod"],
                    unixFrom,
                    unixTo
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseBlockChain((JArray)jResponse["blocks"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Block>> GetWalletBlocksAsync(string walletKey)
        {
            try
            {
                string url = string.Format(Address + endpoints["blocksByWallet"],
                    walletKey
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseBlocks((JArray)jResponse["blocks"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsAsync(string walletKey)
        {
            try
            {
                string url = string.Format(Address + endpoints["transactionsWithWallet"],
                    walletKey
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsAsync(string fromWalletKey, string toWalletKey)
        {
            try
            {
                string url = string.Format(Address + endpoints["transactionsFromToWallet"],
                    fromWalletKey,
                    toWalletKey
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsFromAsync(string fromWalletKey)
        {
            try
            {
                string url = string.Format(Address + endpoints["transactionsFromWallet"],
                    fromWalletKey
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsToAsync(string toWalletKey)
        {
            try
            {
                string url = string.Format(Address + endpoints["transactionsToWallet"],
                    toWalletKey
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateTime from)
        {
            try
            {
                long unix = from.ToUnix();
                string url = string.Format(Address + endpoints["transactionsByDate"],
                    unix
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateTime from, DateTime to)
        {
            try
            {
                long unixFrom = from.ToUnix(),
                    unixto = to.ToUnix();

                string url = string.Format(Address + endpoints["transactionsByPeriod"],
                    unixFrom,
                    unixto
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactions(decimal amount)
        {
            try
            {
                string sAmount = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                string url = string.Format(Address + endpoints["transactionsByAmount"],
                    sAmount
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        public async Task<List<Transaction>> GetTransactions(decimal minimum, decimal maximum)
        {
            try
            {
                string sMinimum = minimum.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                    sMaximum = maximum.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

                string url = string.Format(Address + endpoints["transactionsWithinAmount"],
                    sMinimum,
                    sMaximum
                );

                RestResponse response = await consumer.GetAsync(new Uri(url));

                JObject jResponse = response.JsonBody;
                return ParseTransactions((JArray)jResponse["transactions"]);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Unable to parse REST response (malformed JSON).", exc);
            }
            catch (ArgumentNullException exc)
            {
                throw new RestException("Please provide a valid Uri.", exc);
            }
            catch (HttpRequestException exc)
            {
                throw new RestException(exc.Message, exc);
            }
            catch (WebException exc)
            {
                throw new RestException(exc.Message, exc);
            }
        }

        #endregion

        #region Result Parsing

        private List<Block> ParseBlocks(JArray blockArray)
        {
            List<Block> blocks = new List<Block>(blockArray.Count);
            foreach (JToken bt in blockArray)
            {
                Block block = bt.ToObject<Block>();
                blocks.Add(block);
            }

            return blocks;
        }

        private LinkedList<Block> ParseBlockChain(JArray blockArray)
        {
            LinkedList<Block> blocks = new LinkedList<Block>();
            foreach (JToken bt in blockArray)
            {
                Block block = bt.ToObject<Block>();
                blocks.AddLast(block);
            }

            return blocks;
        }

        private List<Transaction> ParseTransactions(JArray transactionsArray)
        {
            List<Transaction> transactions = new List<Transaction>(transactionsArray.Count);
            foreach (JToken tt in transactionsArray)
            {
                Transaction transaction = tt.ToObject<Transaction>();
                transactions.Add(transaction);
            }

            return transactions;
        }

        #endregion
    }
}
