using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
	public partial class ShopPage : ContentPage
	{
		private int currentItemCount;
        private bool loadFirstTime;
        RestConsumer restConsumer;
        Uri baseUrl;

		public ShopPage()
		{
			InitializeComponent();
            
            BindingContext = this;
            restConsumer = new RestConsumer();

			currentItemCount = 0;
			loadFirstTime = true;
		}

        /// <summary>
        /// Fetch rest server ip and public key from storage.
        /// </summary>
        private async void getStoredValues()
        {
            string defaultIp = "84.29.78.31:8081";

            try
            {
                object shopServerIpObj = null;

                bool fetchedShopServerIp = Application.Current.Properties.TryGetValue("shopip", out shopServerIpObj);

                string shopServerIp = "";

                if (fetchedShopServerIp)
                {
                    shopServerIp = shopServerIpObj.ToString();
                }
                else
                {
                    Application.Current.Properties.Add(new KeyValuePair<string, object>("shopip", defaultIp));

                    shopServerIp = defaultIp;
                }

                baseUrl = new Uri(string.Format("http://{0}/product/get/", shopServerIp));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", string.Format("{0} - {1}", ex.GetType(), ex.Message), "OK");
            }
        }

        private async void loadAllItems()
        {
            this.IsBusy = true;
            int itemIndex = 0;

            getStoredValues();
            List<ShopItem> shopItems = await fetchShopItemsFromRest();

            for (int i = 0; i < shopItems.Count; i++)
            {
                RowDefinition definition = new RowDefinition();
                definition.Height = 140;

                gridShopItems.RowDefinitions.Add(definition);

                for (int j = 0; j < 2; j++)
                {
                    if (itemIndex == shopItems.Count)
                    {
                        this.IsBusy = false;
                        return;
                    }

                    ShopItem item = shopItems[itemIndex];

                    Frame parentFrame = new Frame
                    {
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = new Command (()=>viewShopItem(item.Id))
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
                    lblTitle.Text = item.Name;
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
                    lblPrice.Text = item.Price.ToString();

                    Image imgCoin = new Image();
                    imgCoin.Source = "icon.png";
                    imgCoin.WidthRequest = 15;

                    productPriceStackLayout.Children.Add(lblPrice);
                    productPriceStackLayout.Children.Add(imgCoin);

                    stackLayout.Children.Add(productPriceStackLayout);

                    parentFrame.Content = stackLayout;
                    gridShopItems.Children.Add(parentFrame, j, i);

                    itemIndex++;
                }
            }

            this.IsBusy = false;
        }

        /// <summary>
        /// Loads next shop items.
        /// </summary>
		private async void loadNextItems()
		{
			int tempCounter = currentItemCount;

			this.IsBusy = true;

            getStoredValues();
            List<ShopItem> shopItems = await fetchShopItemsFromRest();

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
								Command = new Command (()=>viewShopItem(-1))
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
		async void viewShopItem(int productId)
		{
            await this.Navigation.PushAsync(new ShopItemPage(productId));
		}

        private async Task<List<ShopItem>> fetchShopItemsFromRest()
		{
			return await Task.Run(async () => 
            {
                try
                {
                    RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, "all"));

                    JArray jsonArray = JArray.Parse(response.PlainBody);

                    List<ShopItem> itemList = jsonArray.ToObject<List<ShopItem>>();
                    return itemList;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Oops, something went wrong", "OK");
                    return null;
                }
			});
		}

        /// <summary>
        /// Checks if the shop items should be loaded for the first time.
        /// </summary>
        public void LoadFirstItems()
		{
			if (loadFirstTime)
			{
                loadAllItems();
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
