using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluckurWallet.ServiceLayer.Rest
{
    public class RestResponse
    {
        /// <summary>
        /// Gets the Http response returned by the REST service.
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <summary>
        /// Gets a value indicating whether the REST service responded with a successful status code.
        /// </summary>
        public bool Successful => Response != null ? Response.IsSuccessStatusCode : false;

        private JObject _body;
        /// <summary>
        /// Attempts to parse and return <see cref="PlainBody"/> as a <see cref="JObject"/>.
        /// </summary>
        /// <exception cref="JsonReaderException"/>
        public JObject JsonBody
        {
            get
            {
                if (_body == null)
                {
                    _body = JObject.Parse(PlainBody);
                }
                return _body;
            }
        }

        /// <summary>
        /// Gets the plain text body returned in the response.
        /// </summary>
        public string PlainBody { get; set; }

        /// <summary>
        /// Instantiates a new REST response.
        /// </summary>
        /// <param name="httpResponse">REST response.</param>
        /// <param name="content">Response text (body).</param>
        public RestResponse(HttpResponseMessage httpResponse, string content)
        {
            Response = httpResponse;
            PlainBody = content;
        }
    }
}
