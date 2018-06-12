using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class HelpPage : ContentPage
    {
        public HelpPage()
        {
            InitializeComponent();

			getDebugInfo();
        }

		private async void getDebugInfo()
		{
			try
			{
				object publicKeyObj = null;
				object privateKeyObj = null;
				object quizServerIpObj = null;
                object explorerServerObj = null;

				bool fetchedPublicKey = Application.Current.Properties.TryGetValue("pubKey", out publicKeyObj);
				bool fetchedPrivateKey = Application.Current.Properties.TryGetValue("privKey", out privateKeyObj);
				bool fetchedQuizServerIp = Application.Current.Properties.TryGetValue("quizip", out quizServerIpObj);
                bool fetchedExplorerServer = Application.Current.Properties.TryGetValue("explorerServer", out explorerServerObj);

				string publicKey = fetchedPublicKey ? publicKeyObj.ToString() : "???";
				string privateKey = fetchedPrivateKey ? privateKeyObj.ToString() : "???";
				string quizServerIp = fetchedQuizServerIp ? quizServerIpObj.ToString() : "???";
                string explorerServer = fetchedExplorerServer ? explorerServerObj.ToString() : "???";

				entPublicKey.Text = publicKey;
				entPrivateKey.Text = privateKey;
				entQuizServerIp.Text = quizServerIp;
                entExplorerServer.Text = explorerServer;
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", string.Format("{0} - {1}", ex.GetType(), ex.Message), "OK");
			}
		}

		private async void btnSaveData_Click(object sender, EventArgs e)
		{
			try
			{
				if (entPublicKey.Text == string.Empty || entPrivateKey.Text == string.Empty || entQuizServerIp.Text == string.Empty)
                {
                    await DisplayAlert("Could not save", "All fields must be filled in.", "OK");
                }

                Application.Current.Properties.Clear();

				Application.Current.Properties.Add(new KeyValuePair<string, object>("pubKey", entPublicKey.Text));
				Application.Current.Properties.Add(new KeyValuePair<string, object>("privKey", entPrivateKey.Text));
				Application.Current.Properties.Add(new KeyValuePair<string, object>("quizip", entQuizServerIp.Text));
                Application.Current.Properties.Add(new KeyValuePair<string, object>("explorerServer", entExplorerServer.Text));
                await Application.Current.SavePropertiesAsync();

                // Because Application.Current.PropertyChanged doesn't fire on the above changed, for now we'll change the Explorer address manually.
                ServiceLayer.BlockchainExplorer.Address = entExplorerServer.Text;

                await DisplayAlert("Success", "Data saved!", "OK");
			}
			catch (Exception ex)
            {
                await DisplayAlert("Error", string.Format("{0} - {1}", ex.GetType(), ex.Message), "OK");
            }
        }

		private void btnResetForm_Click(object sender, EventArgs e)
        {
			getDebugInfo();
        }
    }
}
