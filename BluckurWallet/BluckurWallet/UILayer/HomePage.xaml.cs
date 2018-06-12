using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class HomePage : TabbedPage
    {
        public HomePage()
        {
            InitializeComponent();

			this.CurrentPageChanged += (object sender, EventArgs e) => 
			{
                Page page = this.CurrentPage;
				if (page is ShopPage)
				{
					ShopPage shopPage = (ShopPage)page;
					shopPage.LoadFirstItems();
				}
            };
        }

        /// <summary>
        /// Navigate to NewTransactionPage page.
        /// </summary>
        async void btnStartQuiz_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new QuizPage());
        }
    }
}