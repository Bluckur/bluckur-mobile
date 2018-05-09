using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluckurWallet.UILayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlockchainExplorerPage : ContentPage
    {
        public BlockchainExplorerPage()
        {
            InitializeComponent();
            SelectDay(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
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

            // GET BLOCKS OF DAY
            var blocks = new string[144];
            await ShowBlocks(blocks, day);
        }

        private async Task ShowBlocks(object[] blocks, DateTime day)
        {
            int row = 0;
            int column = 0;

            gridBlocks.RowDefinitions.Clear();
            gridBlocks.Children.Clear();


            foreach (var block in blocks)
            {
                DateTime time = day.AddMinutes(10 * row);

                gridBlocks.RowDefinitions.Add(new RowDefinition()
                {
                    Height = 40
                });

                // Add time
                Label label = new Label();
                label.Text = time.ToString("HH:mm");
                label.VerticalTextAlignment = TextAlignment.Center;
                label.HorizontalTextAlignment = TextAlignment.Center;
                gridBlocks.Children.Add(label, 0, row);

                // Add button
                Button button = new Button();
                button.Text = "Click me";
                button.Clicked += async (sender, e) =>
                {
                    await DisplayAlert("Block", "There's no data to display yet.", "KThanks");
                };
                gridBlocks.Children.Add(button, 1, row);

                row++;

                await Task.Delay(10);
            }
        }
    }
}