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

			shopItem = new ShopItem(productId, 10, "ic_shop.png", "Product Name", "Lorem ipsum enzo", 5);
			loadItem();
		}

        private void loadItem()
		{
			lblProductName.Text = shopItem.Name;
			imgProductImage.Source = shopItem.ImagePath;
			lblDescription.Text = shopItem.Description;

            // In Stock
			if (shopItem.Stock > 0)
			{
				lblInStock.Text = "In Stock";
				lblInStock.TextColor = Color.FromHex("5cb85c");
			}
			else
			{
                lblInStock.Text = "Out of Stock";
				lblInStock.TextColor = Color.FromHex("d9534f");
			}
		}
	}
}
