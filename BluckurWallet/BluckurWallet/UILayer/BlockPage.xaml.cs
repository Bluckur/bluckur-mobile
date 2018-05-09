using BluckurWallet.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluckurWallet.UILayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlockPage : ContentPage
    {
        public BlockPage(Block block)
        {
            InitializeComponent();

            ShowTransactions(block.Transactions);
        }

        private async void ShowTransactions(ICollection<Transaction> transactions)
        {
            int row = 0;

            foreach (var item in transactions)
            {
                gridTransactions.RowDefinitions.Add(new RowDefinition()
                {
                    Height = 30
                });

                DateTime time = new DateTime(1970, 1, 1);
                time = time.AddSeconds(item.TimeStamp);

                Label from = new Label();
                Label amount = new Label();
                Label to = new Label();
                from.HorizontalTextAlignment = TextAlignment.Center;
                amount.HorizontalTextAlignment = TextAlignment.Center;
                to.HorizontalTextAlignment = TextAlignment.Center;
                from.VerticalTextAlignment = TextAlignment.Center;
                amount.VerticalTextAlignment = TextAlignment.Center;
                to.VerticalTextAlignment = TextAlignment.Center;

                from.Text = item.Sender != null ? item.Sender : "Nobody";
                amount.Text = string.Format("=> {0} {1} =>", item.Amount, item.Type);
                to.Text = item.Recipient;

                gridTransactions.Children.Add(from, 0, row);
                gridTransactions.Children.Add(amount, 1, row);
                gridTransactions.Children.Add(to, 2, row);

                row++;

                await Task.Delay(10);
            }
        }
    }
}