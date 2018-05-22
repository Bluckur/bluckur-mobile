using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class ShopPage : ContentPage
    {
		private int currentItemCount;

        public ShopPage()
        {
            InitializeComponent();

			currentItemCount = 0;

			loadNextItems();
        }

        private void loadNextItems()
		{
			int tempCounter = currentItemCount;

			for (int i = tempCounter; i < tempCounter + 5; i++)
            {
                RowDefinition definition = new RowDefinition();
                definition.Height = 140;

                gridShopItems.RowDefinitions.Add(definition);

                for (int j = 0; j < 2; j++)
                {
					Frame parentFrame = new Frame()
					{
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = new Command (()=>viewShopItem()),
                            }
                        }
                    };

					parentFrame.CornerRadius = 5;
					parentFrame.BackgroundColor = Color.White;
					
					// Parent stacklayout
					StackLayout stackLayout = new StackLayout();

                    LayoutOptions optionsContentCenter = new LayoutOptions(LayoutAlignment.Center, false);
					stackLayout.HorizontalOptions = optionsContentCenter;
					stackLayout.Orientation = StackOrientation.Vertical;

                    // Product title and image.
                    Label lblTitle = new Label();
                    lblTitle.Text = "Product Name";

                    Image imgItem = new Image();
					imgItem.Source = "ic_shop.png";

                    stackLayout.Children.Add(lblTitle);
                    stackLayout.Children.Add(imgItem);
                    
					// Product price
					StackLayout productPriceStackLayout = new StackLayout();
					productPriceStackLayout.Orientation = StackOrientation.Horizontal;
					productPriceStackLayout.HorizontalOptions = optionsContentCenter;

                    Label lblPrice = new Label();
					lblPrice.Text = "0";

                    Image imgCoin = new Image();
					imgCoin.Source = "icon.png";
					imgCoin.WidthRequest = 15;

					productPriceStackLayout.Children.Add(lblPrice);
					productPriceStackLayout.Children.Add(imgCoin);

					stackLayout.Children.Add(productPriceStackLayout);
                    
					parentFrame.Content = stackLayout;
					gridShopItems.Children.Add(parentFrame, j, i);
                }

				currentItemCount = i;
            }

			currentItemCount++;
		}

		void btnMoreItems_Click(object sender, System.EventArgs e)
		{
			loadNextItems();
		}

		/// <summary>
        /// Navigate to ShopItem page.
        /// </summary>
        async void viewShopItem()
        {
			await this.Navigation.PushAsync(new ShopItemPage());
        }
    }
}
