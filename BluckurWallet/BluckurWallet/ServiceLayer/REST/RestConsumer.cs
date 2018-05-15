using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BluckurWallet.ServiceLayer.Rest
{
    public class RestConsumer
    {
        /// <summary>
        /// Maximum amount of bytes in the REST response.
        /// 16777216 = 16MB.
        /// </summary>
        public static readonly long MAX_SIZE = 16777216;

        private HttpClient client;
        /// <summary>
        /// Instantiates a new REST consumer.
        /// Can be used to send send or post HTTP requests to a REST service.
        /// The maximum 
        /// </summary>
        public RestConsumer()
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 5);
            client.MaxResponseContentBufferSize = MAX_SIZE; // 2^24 (16MB)
        }

        /// <summary>
        /// Sends a GET request and returns the response.
        /// </summary>
        /// <param name="target">REST Uri.</param>
        /// <returns>REST response.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="WebException"/>
        public async Task<RestResponse> GetAsync(Uri target)
        {
            var response = await client.GetAsync(target);
            var jsonContent = await response.Content.ReadAsStringAsync();

            return new RestResponse(response, jsonContent);
        }

        /// <summary>
        /// Sends a POST request and returns the response.
        /// </summary>
        /// <param name="target">REST Uri.</param>
        /// <param name="body">REST Response.</param>
        /// <returns>REST Response.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="WebException"/>
        public async Task<RestResponse> PostAsync(Uri target, JObject body)
        {
            var json = body.ToString(Newtonsoft.Json.Formatting.None);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(target, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new RestResponse(response, responseContent);
        }
    }
}
