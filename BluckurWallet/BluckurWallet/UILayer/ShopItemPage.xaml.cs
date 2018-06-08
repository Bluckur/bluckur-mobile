using System;
using System.Collections.Generic;
using BluckurWallet.Domain;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
	public partial class ShopItemPage : ContentPage
	{
		private ShopItem shopItem;

		public ShopItemPage(int productId)
		{
			InitializeComponent();

			shopItem = new ShopItem(productId, 10.15, "ic_shop.png", "Product Name", "Lorem ipsum enzo", 5);
			loadItem();
		}

        private void loadItem()
		{
			lblProductName.Text = shopItem.Name;
			imgProductImage.Source = shopItem.ImagePath;
			lblDescription.Text = shopItem.Description;
            lblPrice.Text = shopItem.Price.ToString().Replace('.', ',');

            // In Stock
			if (shopItem.Stock > 0)
			{
                lblInStock.Text = shopItem.Stock + " items in stock";
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
            if (entAmount.Text == string.Empty)
            {
                await DisplayAlert("Wrong input", "Oops, you can't order 0 items ;)", "OK");
                return;
            }

            int amount = Convert.ToInt32(entAmount.Text);

            if (amount > shopItem.Stock)
            {
                await DisplayAlert("Not enough items", "Oops, not enough items in stock.", "OK");
                return;
            }

            ShoppingCart.GetShoppingCart().shopItems.Add(shopItem, amount);
            await DisplayAlert("Success!", amount + " x " + shopItem.Name + " added to cart!", "OK");
        }
	}
}
