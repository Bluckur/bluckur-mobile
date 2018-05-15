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
                definition.Height = 120;

                gridShopItems.RowDefinitions.Add(definition);

                for (int j = 0; j < 2; j++)
                {
                    StackLayout stackLayout = new StackLayout();
                    LayoutOptions options = new LayoutOptions(LayoutAlignment.Center, false);
                    stackLayout.HorizontalOptions = options;
					//stackLayout.Spacing = 2;
					stackLayout.BackgroundColor = Color.Red;

                    Label lblTitle = new Label();
                    lblTitle.Text = "Product Name";

                    Image imgItem = new Image();
                    imgItem.Source = "icon.png";

                    stackLayout.Children.Add(lblTitle);
                    stackLayout.Children.Add(imgItem);

                    gridShopItems.Children.Add(stackLayout, j, i);
                }

				currentItemCount = i;
            }
		}

		void btnMoreItems_Click(object sender, System.EventArgs e)
		{
			loadNextItems();
		}
    }
}
