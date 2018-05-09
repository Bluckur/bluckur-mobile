using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluckurWallet.UILayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestPage : ContentPage
    {
        RestConsumer consumer;

        public RestPage()
        {
            InitializeComponent();
            consumer = new RestConsumer();
        }

        private void Send_Clicked(object sender, EventArgs e)
        {
            Uri uri;
            try
            {
                uri = new Uri(tbxUriGet.Text);
            }
            catch (ArgumentNullException)
            {
                tbxGetReponse.Text = "Please enter an Uri.";
                return;
            }
            catch (UriFormatException)
            {
                tbxGetReponse.Text = "Invalid Uri.";
                return;
            }

            HandleGet(uri);
        }

        private async void HandleGet(Uri uri)
        {
            try
            {
                var response = await consumer.GetAsync(uri);
                tbxGetReponse.Text = response.JsonBody.ToString(Formatting.Indented);
            }
            catch (HttpRequestException exc)
            {
                tbxGetReponse.Text = "Request failed: " + exc.Message;
            }
            catch (WebException exc)
            {
                tbxPostResponse.Text = "Request failed: " + exc.Message;
            }
            catch (JsonReaderException exc)
            {
                tbxGetReponse.Text = "Couldn't parse response as JSON: " + exc.Message;
            }
        }

        private void Post_Clicked(object sender, EventArgs e)
        {
            Uri uri;
            try
            {
                uri = new Uri(tbxUriPost.Text);
            }
            catch (ArgumentNullException)
            {
                tbxPostResponse.Text = "Please enter an Uri.";
                return;
            }
            catch (UriFormatException)
            {
                tbxPostResponse.Text = "Invalid Uri.";
                return;
            }

            JObject data;
            try
            {
                data = JObject.Parse(tbxPostData.Text);
            }
            catch (ArgumentNullException)
            {
                tbxPostResponse.Text = "Please enter a POST body.";
                return;
            }
            catch (JsonReaderException exc)
            {
                tbxPostResponse.Text = "Couldn't parse data: " + exc.Message;
                return;
            }

            HandlePost(uri, data);
        }

        private async void HandlePost(Uri uri, JObject data)
        {
            RestResponse response;
            try
            {
                response = await consumer.PostAsync(uri, data);
            }
            catch (HttpRequestException exc)
            {
                tbxPostResponse.Text = "Request failed: " + exc.Message;
                return;
            }
            catch (WebException exc)
            {
                tbxPostResponse.Text = "Request failed: " + exc.Message;
                return;
            }

            try
            {
                string json = response.JsonBody.ToString(Formatting.Indented);
                tbxPostResponse.Text = json;
            }
            catch
            {
                tbxPostResponse.Text = response.PlainBody;
            }
        }
    }
}