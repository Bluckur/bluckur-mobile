using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluckurWallet.ServiceLayer.Rest
{
    public class RestResponse
    {
        public HttpResponseMessage Response { get; }

        public bool Successful => Response != null ? Response.IsSuccessStatusCode : false;

        private JObject _body;
        /// <summary>
        /// 
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

        public string PlainBody { get; set; }

        public RestResponse(HttpResponseMessage httpResponse, string content)
        {
            Response = httpResponse;
            PlainBody = content;
        }
    }
}
