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

namespace BluckurWallet.ServiceLayer
{
    /// <summary>
    /// A helper class to obtain data from the REST blockchain explorer service.
    /// The class doesn't consume exceptions, but they are wrapped in a <see cref="RestException"/> for easier parsing.
    /// </summary>
    public class BlockchainExplorer
    {
        // TODO: Configuration for endpoint (being worked on in another branch).
        private static readonly string endpoint = "http://silvermods.com/meekel/api/Rest/";
        private static readonly string dateFormat = "yyyy-MM-dd";

        private static readonly Dictionary<string, string> endpoints = new Dictionary<string, string>()
        {
            { "blockByHash",                "block/byhash/{0}" },
            { "nextBlock",                  "block/next/{0}" },
            { "previousBlock",              "block/previous/{0}" },
            { "blocksByDate",               "block/date/{0}" },
            { "blocksByPeriod",             "block/period/{0}/{1}" },
            { "blocksByWallet",             "block/wallet/{0}" },
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
        }

        /// <summary>
        /// Gets all blocks in the blockchain added in the given day (based on UTC time).
        /// </summary>
        /// <param name="day">Day.</param>
        /// <returns>Blocks of the day.</returns>
        /// <exception cref="RestException"/>
        public async Task<List<Block>> GetBlocksAsync(DateTime day)
        {
            try
            {
                long unix = day.ToUnix();
                RestResponse response = await consumer.GetAsync(new Uri(endpoint, "BlocksFrom/" + unix));

                // TODO: This will soon be a JObject.
                JArray jBlocks = JArray.Parse(response.PlainBody);
                return ParseBlocks(jBlocks);
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
        public async Task<List<Block>> GetBlocksAsync(DateTime from, DateTime to)
        {
            try
            {
                long unixFrom = from.ToUnix();
                long unixTo = to.ToUnix();

                RestResponse response = await consumer.GetAsync(new Uri(endpoint, "BlocksFrom/" + unixFrom + "/" + unixTo));

                // TODO: This will soon be a JObject.
                JArray jBlocks = JArray.Parse(response.PlainBody);
                return ParseBlocks(jBlocks);
            }
            catch (JsonReaderException exc)
            {
                throw new RestException("Response is not a JSON array.", exc);
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

        public async Task<Block> GetBlock(string hash)
        {
            try
            {
                // TODO: Verify hash (hex of length x?)
                RestResponse response = await consumer.GetAsync(new Uri(endpoint, "Block/" + hash));
                JObject jBlock = response.JsonBody;
                return jBlock.ToObject<Block>();
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

        public async Task<Block> NextBlock(Block block)
        {

        }

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
    }
}
