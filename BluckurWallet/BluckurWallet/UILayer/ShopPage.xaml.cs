using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluckurWallet.Domain;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
	public partial class ShopPage : ContentPage
	{
		private int currentItemCount;
		private bool loadFirstTime;

		public ShopPage()
		{
			InitializeComponent();
            
			BindingContext = this;

			currentItemCount = 0;
			loadFirstTime = true;
		}

        /// <summary>
        /// Loads next shop items.
        /// </summary>
		private async void loadNextItems()
		{
			int tempCounter = currentItemCount;

			this.IsBusy = true;

			List<ShopItem> shopItems = await fetchShopItems();

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
								Command = new Command (()=>viewShopItem())
							}
						}
					};

					parentFrame.CornerRadius = 5;
					parentFrame.Padding = 5;
					parentFrame.BackgroundColor = Color.White;
                    
					// Parent stacklayout
					StackLayout stackLayout = new StackLayout();
                      
					stackLayout.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
					stackLayout.Orientation = StackOrientation.Vertical;

					// Product title and image.
					Label lblTitle = new Label();
					lblTitle.Text = "Product Name";
					lblTitle.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false);

					Image imgItem = new Image();
					imgItem.Source = "ic_shop.png";
					imgItem.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, true);

					stackLayout.Children.Add(lblTitle);
					stackLayout.Children.Add(imgItem);

					// Product price
					StackLayout productPriceStackLayout = new StackLayout();
					productPriceStackLayout.Orientation = StackOrientation.Horizontal;
					productPriceStackLayout.HorizontalOptions = new LayoutOptions(LayoutAlignment.End, false);
                    
					Label lblPrice = new Label();
					lblPrice.Text = "0,00";

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

            this.IsBusy = false;
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
			await this.Navigation.PushAsync(new ShopItemPage(-1));
		}

        private async Task<List<ShopItem>> fetchShopItems()
		{
			return await Task.Run(async () => 
			{
				List<ShopItem> shopItems = new List<ShopItem>();

                for (int i = 0; i < 10; i++)
                {
                    shopItems.Add(new ShopItem(i, 2, "ic_shop.png", "Product name", "You should buy this!", 10));
                }

				return shopItems;
			});
		}

        /// <summary>
        /// Checks if the shop items should be loaded for the first time.
        /// </summary>
        public void LoadFirstItems()
		{
			if (loadFirstTime)
			{
                loadNextItems();
				loadFirstTime = false;
			}
		}

        protected override void OnAppearing()
        {
            updateCartInfo();      
        }

        private void updateCartInfo()
        {
            Tuple<string, int> tuple = ShoppingCart.GetShoppingCart().GetTotalPriceAndItemCount();
            string price = tuple.Item1;
            int itemCount = tuple.Item2;

            if (itemCount <= 0)
            {
                btnCartPage.IsVisible = false;
                return;
            }

            btnCartPage.IsVisible = true;

            btnCartPage.Text = itemCount + " items in cart (" + price + ") ->";
        }

        async void btnCartPage_Click(object sender, System.EventArgs e)
        {
            await this.Navigation.PushAsync(new CartPage());
        }
	}
}
