using System;
using System.Diagnostics;
using BluckurWallet.Domain;
using BluckurWallet.UILayer;
using Xamarin.Forms;

namespace BluckurWallet
{
	public partial class App : Application
	{
		public static WalletData WalletData { get; private set; }

        public App()
        {
			InitializeComponent();
                MainPage = new NavigationPage(new HomePage());  

			getWalletData();

			if (WalletData == null)
			{
				MainPage = new NavigationPage(new SetupPage());
			}
			else
			{
				
                MainPage = new NavigationPage(new HomePage());  
			}
        }

        private void getWalletData()
		{
			try
			{
				object publicKeyObj = null;
				object privateKeyObj = null;

				bool fetchedPublicKey = Application.Current.Properties.TryGetValue("pubkey", out publicKeyObj);
                bool fetchedPrivateKey = Application.Current.Properties.TryGetValue("privkey", out publicKeyObj);
                
				if (!fetchedPublicKey || !fetchedPrivateKey)
				{
					return;
				}

				string publicKey = publicKeyObj.ToString();
				string privateKey = privateKeyObj.ToString();

				WalletData walletData = new WalletData(publicKey, privateKey);

				WalletData = walletData;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
