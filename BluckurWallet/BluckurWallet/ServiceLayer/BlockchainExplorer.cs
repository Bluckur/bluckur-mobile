using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluckurWallet.ServiceLayer
{
    public class BlockchainExplorer
    {
        private static readonly Uri endpoint = new Uri("http://silvermods.com/meekel/api/Rest/");

        RestConsumer consumer;

        public BlockchainExplorer()
        {
            consumer = new RestConsumer();
        }

        public async Task<List<Block>> GetBlocksAsync(DateTime from)
        {
            string date = from.ToString("yyyy-MM-dd");
            RestResponse response = await consumer.GetAsync(new Uri(endpoint, "BlocksFrom/" + date));

            JArray jBlocks = JArray.Parse(response.PlainBody);
            List<Block> blocks = new List<Block>(jBlocks.Count);
            foreach (JToken blockToken in jBlocks)
            {
                Block block = blockToken.ToObject<Block>();
                blocks.Add(block);
            }
            return blocks;
        }
    }
}
