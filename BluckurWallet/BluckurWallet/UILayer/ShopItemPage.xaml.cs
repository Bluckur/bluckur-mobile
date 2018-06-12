using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer.Rest;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
	public partial class ShopItemPage : ContentPage
	{
        private ShopItem shopItem;
        RestConsumer restConsumer;
        Uri baseUrl;

		public ShopItemPage(int productId)
		{
            InitializeComponent();

            restConsumer = new RestConsumer();

            setupPage(productId);
		}

        private async void setupPage(int productId)
        {
            await getStoredValues();
            await fetchShopItemFromRest(productId);
            loadItem();
        }

        /// <summary>
        /// Fetch rest server ip and public key from storage.
        /// </summary>
        private async Task getStoredValues()
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

        private async Task fetchShopItemFromRest(int productId)
        {
            try
            {
                RestResponse response = await restConsumer.GetAsync(new Uri(baseUrl, productId.ToString()));

                JObject json = response.JsonBody;

                ShopItem item = json.ToObject<ShopItem>();
                shopItem = item;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops, something went wrong", "OK");
            }
        }

        private void loadItem()
		{
            if (shopItem == null)
            {
                return;
            }

			lblProductName.Text = shopItem.Name;
			imgProductImage.Source = "ic_shop.png";
			lblDescription.Text = shopItem.Description;
            lblPrice.Text = shopItem.Price.ToString().Replace('.', ',');

            // In Stock
			if (shopItem.Quantity > 0)
			{
                lblInStock.Text = shopItem.Quantity + " items in stock";
				lblInStock.TextColor = Color.FromHex("5cb85c");
			}
			else
			{
                lblInStock.Text = "Out of Stock";
				lblInStock.TextColor = Color.FromHex("d9534f");
			}
		}

        async void btnAddToCart_Click(object sender, System.EventArgs e)
        {
            if (shopItem == null)
            {
                return;
            }

            if (entAmount.Text == string.Empty)
            {
                await DisplayAlert("Wrong input", "Oops, you can't order 0 items ;)", "OK");
                return;
            }

            int amount = Convert.ToInt32(entAmount.Text);

            if (amount > shopItem.Quantity)
            {
                await DisplayAlert("Not enough items", "Oops, not enough items in stock.", "OK");
                return;
            }

            ShoppingCart.GetShoppingCart().shopItems.Add(shopItem, amount);
            await DisplayAlert("Success!", amount + " x " + shopItem.Name + " added to cart!", "OK");
        }
	}
}
