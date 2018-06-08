using System;
using System.Collections.Generic;
using BluckurWallet.Domain;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public partial class CartPage : ContentPage
    {
        public CartPage()
        {
            InitializeComponent();

            loadItems();
        }


        /// <summary>
        /// Load items.
        /// </summary>
        private void loadItems()
        {
            Dictionary<ShopItem, int> shopItems = ShoppingCart.GetShoppingCart().shopItems;
            int i = 0;

            foreach (KeyValuePair<ShopItem, int> pair in shopItems)
            {
                ShopItem currentItem = pair.Key;

                RowDefinition definition = new RowDefinition();
                definition.Height = 140;

                gridItems.RowDefinitions.Add(definition);

                Frame parentFrame = new Frame();
                parentFrame.CornerRadius = 5;
                parentFrame.Padding = 5;
                parentFrame.BackgroundColor = Color.White;

                // Parent stacklayout
                StackLayout stackLayout = new StackLayout();

                stackLayout.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
                stackLayout.Orientation = StackOrientation.Vertical;

                // Product title and image.
                Label lblTitle = new Label();
                lblTitle.Text = currentItem.Name;
                lblTitle.HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, false);

                Image imgItem = new Image();
                imgItem.Source = currentItem.ImagePath;
                imgItem.VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, true);

                stackLayout.Children.Add(lblTitle);
                stackLayout.Children.Add(imgItem);

                // Product price
                StackLayout productPriceStackLayout = new StackLayout();
                productPriceStackLayout.Orientation = StackOrientation.Horizontal;
                productPriceStackLayout.HorizontalOptions = new LayoutOptions(LayoutAlignment.End, false);

                Label lblPrice = new Label();
                double totalItemPrice = currentItem.Price * pair.Value;
                lblPrice.Text = totalItemPrice.ToString().Replace('.', ',');

                Image imgCoin = new Image();
                imgCoin.Source = "icon.png";
                imgCoin.WidthRequest = 15;

                productPriceStackLayout.Children.Add(lblPrice);
                productPriceStackLayout.Children.Add(imgCoin);

                stackLayout.Children.Add(productPriceStackLayout);

                parentFrame.Content = stackLayout;
                gridItems.Children.Add(parentFrame, 0, i);
                i++;
            }
        }

        void btnOrder_Click(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
