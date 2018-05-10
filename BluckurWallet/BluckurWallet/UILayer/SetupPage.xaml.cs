using System;
using System.Collections.Generic;
using BluckurWallet.Domain;
using BluckurWallet.Util;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace BluckurWallet.UILayer
{
    public partial class SetupPage : ContentPage
    {
		
        public SetupPage()
		{
            InitializeComponent();
        }

		private async void btnScanQr_Click(object sender, EventArgs e)
		{         
            // Create scanner page.
            ZXingScannerPage scanPage = QRCodeUtil.CreateScannerPage();

            // Handle scanner result.
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
					parseWalletData(result.Text);
                });
            };

            // Navigate to scanner page
            await Navigation.PushAsync(scanPage);         
		}

        /// <summary>
        /// Tries to parse and save wallet data. Navigates to main app if succeeded.
        /// </summary>
        /// <param name="keysJson">Json string containing keys.</param>
        private async void parseWalletData(string keysJson)
		{
			try
			{
                if (keysJson == string.Empty)
                {
                    await DisplayAlert("Error", "Oops, something went wrong", "OK");
                    return;
                }

                JObject jObject = JObject.Parse(keysJson);

                WalletData walletData = new WalletData(jObject.GetValue("publicKey").ToString(), jObject.GetValue("privateKey").ToString());

                Application.Current.Properties.Add(new KeyValuePair<string, object>("pubKey", walletData.PublicKey));
                Application.Current.Properties.Add(new KeyValuePair<string, object>("privKey", walletData.PublicKey));

                Application.Current.MainPage = new NavigationPage(new HomePage());
			}
			catch (Exception ex)
			{
                await DisplayAlert("Error", "Oops, could not parse your wallet data.", "OK");
			}
		}

		private void btnSkip_Click(object sender, EventArgs e)
		{
            Application.Current.MainPage = new NavigationPage(new HomePage());
		}
    }
}
