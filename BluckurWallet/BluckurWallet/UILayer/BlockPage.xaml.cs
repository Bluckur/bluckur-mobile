using BluckurWallet.Domain;
using BluckurWallet.ServiceLayer;
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
        BlockchainExplorer explorer;

        bool testing = true; // TODO: REMOVE.

        public BlockPage(Block block)
        {
            InitializeComponent();
            explorer = new BlockchainExplorer();

            lblPreviousHash.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (!string.IsNullOrWhiteSpace(block.Header.PreviousHash))
                    {
                        Block b = await explorer.GetBlock(block.Header.PreviousHash);
                        await Navigation.PushAsync(new BlockPage(b));
                    }
                })
            });

            Title = "Block " + block.Header.BlockNumber;
            Show(block);
        }

        private async void Show(Block block)
        {
            // HEADER
            var h = block.Header;
            lblNumber.Text = h.BlockNumber.ToString();
            lblHash.Text = h.Hash;
            lblPreviousHash.Text = string.IsNullOrWhiteSpace(h.PreviousHash) ? "Unknown" : h.PreviousHash;
            lblReward.Text = h.Reward.ToString();
            lblTime.Text = h.TimeStamp.ToDateTime().ToString("dd-MM-yyyy HH:mm");
            lblValidator.Text = h.Validator;

            // TX
            if (block.Transactions == null)
            {
                block = await explorer.GetBlock(block.Header.Hash);
            }

            if (testing && block.Transactions.Count == 0)
            {
                block.Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = 1.0f,
                        Sender = "abc",
                        Recipient = "def",
                        TimeStamp = DateTime.Now.ToUnix(),
                        Type = "COIN"
                    },
                    new Transaction()
                    {
                        Amount = 5.0f,
                        Sender = "admin",
                        Recipient = "abc",
                        TimeStamp = DateTime.Now.AddMinutes(1).ToUnix(),
                        Type = "STAKE"
                    },
                    new Transaction()
                    {
                        Amount = 1.25f,
                        Sender = "abc",
                        Recipient = "def",
                        TimeStamp = DateTime.Now.AddMinutes(1).ToUnix(),
                        Type = "COIN"
                    }
                };
                ShowTransactions(block.Transactions);
                return;
            }

            ShowTransactions(block.Transactions);
        }

        private async void ShowTransactions(ICollection<Transaction> transactions)
        {
            int row = 0;

            /*
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
            */
        }

        private async Task LoadMore_Clicked(object sender, EventArgs e)
        {

        }
    }
}