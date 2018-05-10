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
                
				bool fetchedPublicKey = Application.Current.Properties.TryGetValue("pubkey", out publicKeyObj);
				bool fetchedPrivateKey = Application.Current.Properties.TryGetValue("privkey", out publicKeyObj);
				bool fetchedQuizServerIp = Application.Current.Properties.TryGetValue("quizip", out quizServerIpObj);

				string publicKey = fetchedPublicKey ? publicKeyObj.ToString() : "???";
				string privateKey = fetchedPrivateKey ? privateKeyObj.ToString() : "???";
				string quizServerIp = fetchedQuizServerIp ? quizServerIpObj.ToString() : "???";

				entPublicKey.Text = publicKey;
				entPrivateKey.Text = privateKey;
				entQuizServerIp.Text = quizServerIp;
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
