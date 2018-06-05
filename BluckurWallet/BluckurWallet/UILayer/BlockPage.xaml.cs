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

            // Make previous hash tappable.
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
                        Sender = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                        Recipient = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                        TimeStamp = DateTime.Now.ToUnix(),
                        Type = "COIN"
                    },
                    new Transaction()
                    {
                        Amount = 100.11f,
                        Sender = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                        Recipient = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                        TimeStamp = DateTime.Now.AddMinutes(1).ToUnix(),
                        Type = "STAKE"
                    },
                    new Transaction()
                    {
                        Amount = 1111.2501f,
                        Sender = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                        Recipient = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
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

            foreach (var item in transactions)
            {
                stackTransactions.Children.Add(CreateTransactionFrame(item));
            }

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

        private Frame CreateTransactionFrame(Transaction transaction)
        {
            Thickness tZero = new Thickness(0d);

            // Card
            Frame frame = new Frame
            {
                CornerRadius = 5,
                Padding = tZero,
                HeightRequest = 54,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            StackLayout body = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(6, 4),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Grid left = new Grid
            {
                HorizontalOptions = LayoutOptions.Start,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = 1 },
                    new ColumnDefinition { Width = 32 },
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = 1 }
                }
            };

            Label lblTime = new Label
            {
                Text = transaction.TimeStamp.ToDateTime().ToString("HH:mm"),
                FontSize = 14,
                
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            Image imgCurrency = new Image
            {
                Source = transaction.Type == "COIN" ? "icon.png" : "ic_skill.png",
                HeightRequest = 32,
                WidthRequest = 32,
                VerticalOptions = LayoutOptions.Center
            };

            Label lblCurrency = new Label
            {
                Text = transaction.Amount.ToString(),
                FontSize = 18,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.CharacterWrap
            };

            Grid grdWallets = new Grid
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition() { Height = GridLength.Star },
                    new RowDefinition() { Height = GridLength.Star }
                }
            };

            Label lblSender = new Label
            {
                Text = transaction.Sender ?? "No sender",
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            Label lblRecipient = new Label
            {
                Text = transaction.Recipient,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };

            frame.Content = body;

            body.Children.Add(left);
            body.Children.Add(grdWallets);

            BoxView spacer1 = new BoxView
            {
                BackgroundColor = Color.FromRgb(238, 238, 238)
            };
            BoxView spacer2 = new BoxView
            {
                BackgroundColor = Color.FromRgb(238, 238, 238)
            };

            left.Children.Add(lblTime, 0, 0);
            left.Children.Add(spacer1, 1, 0);
            left.Children.Add(imgCurrency, 2, 0);
            left.Children.Add(lblCurrency, 3, 0);
            left.Children.Add(spacer2, 4, 0);
            //left.Children.Add(grdWallets);
            
            grdWallets.Children.Add(lblSender, 0, 0);
            grdWallets.Children.Add(lblRecipient, 0, 1);
            
            return frame;
        }

        private async Task LoadMore_Clicked(object sender, EventArgs e)
        {

        }
    }
}