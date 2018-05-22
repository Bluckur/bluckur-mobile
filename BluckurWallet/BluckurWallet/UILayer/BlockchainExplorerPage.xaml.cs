using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BluckurWallet.ServiceLayer.Rest;
using BluckurWallet.ServiceLayer;
using BluckurWallet.Domain;

namespace BluckurWallet.UILayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlockchainExplorerPage : ContentPage
    {
        BlockchainExplorer explorer;

        public BlockchainExplorerPage()
        {
            InitializeComponent();
            explorer = new BlockchainExplorer();
        }
        
        private void PickDate_Clicked(object sender, EventArgs e)
        {
            datePicker.Focus();
        }

        private void DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime time = datePicker.Date;
            time = new DateTime(time.Year, time.Month, time.Day);

            SelectDay(time);
        }

        private async void SelectDay(DateTime day)
        {
            lblDate.Text = "Date: " + day.ToString("dd-MM-yyyy");

            try
            {
                LinkedList<Block> blocks = await explorer.GetBlocksAsync(day);
                await ShowBlocks(blocks);
            }
            catch (RestException exc)
            {
                await DisplayAlert("Error", exc.InnerException.Message, "Ok");
            }            
        }

        private async Task ShowBlocks(ICollection<Block> blocks)
        {
            int row = 0;

            gridBlocks.RowDefinitions.Clear();
            gridBlocks.Children.Clear();


            foreach (var block in blocks)
            {
                gridBlocks.RowDefinitions.Add(new RowDefinition()
                {
                    Height = 40
                });

                DateTime blockTime = new DateTime(1970, 1, 1);
                blockTime = blockTime.AddSeconds(block.Header.TimeStamp);

                // Add time
                Label label = new Label();
                label.Text = blockTime.ToString("HH:mm");
                label.VerticalTextAlignment = TextAlignment.Center;
                label.HorizontalTextAlignment = TextAlignment.Center;
                gridBlocks.Children.Add(label, 0, row);

                // Add button
                Button button = new Button();
                button.Text = "Click me";
                button.TextColor = Color.White;
                button.BackgroundColor = Color.FromHex("e88506");
                button.Clicked += async (sender, e) =>
                {
                    await this.Navigation.PushAsync(new BlockPage(block));
                };
                gridBlocks.Children.Add(button, 1, row);

                row++;

                await Task.Delay(10);
            }
        }
    }
}