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
        private static readonly Color SPACER_COLOR = Color.FromRgb(238, 238, 238);

        BlockchainExplorer explorer;
        Queue<Transaction> pending;

        bool testing = true; // TODO: REMOVE.

        public BlockPage(Block block)
        {
            InitializeComponent();

            explorer = new BlockchainExplorer();
            pending = new Queue<Transaction>();

            // Make previous hash tappable.
            lblPreviousHash.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(ShowPrevious),
                CommandParameter = block
            });

            // Show block
            Show(block);
        }

        /// <summary>
        /// Shows the previous block in a new <see cref="BlockPage"/>.
        /// </summary>
        /// <param name="oBlock">Current block.</param>
        private async void ShowPrevious(object oBlock)
        {
            Block block = oBlock as Block;

            if (block == null) return;

            if (string.IsNullOrWhiteSpace(block.Header.PreviousHash))
            {
                await DisplayAlert("Error", "The genesis block does not have a previous block.", "OK");
            }
            else
            {
                try
                {
                    Block previousBlock = await explorer.GetBlock(block.Header.PreviousHash);
                    await Navigation.PushAsync(new BlockPage(block));
                }
                catch
                {
                    await DisplayAlert("Error", "Couldn't display previous block.", "OK");
                }
            }
        }

        /// <summary>
        /// Shows the block details and transactions.
        /// </summary>
        /// <param name="block">Block to show.</param>
        private async void Show(Block block)
        {
            // HEADER
            var h = block.Header;
            
            Title = "Block " + h.BlockNumber;

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

            #region Test data

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

            #endregion

            ShowTransactions(block.Transactions);
        }

        /// <summary>
        /// Sets the UI to show the given transactions.
        /// Previous transactions will be cleared.
        /// </summary>
        /// <param name="transactions">Transactions to show.</param>
        private void ShowTransactions(ICollection<Transaction> transactions)
        {
            pending.Clear();
            foreach (var item in transactions)
            {
                pending.Enqueue(item);
            }

            stackTransactions.Children.Clear();
            LoadMore();
        }

        /// <summary>
        /// Creates a frame view displaying the details of a transaction.
        /// The returned value can be added to the <see cref="stackTransactions"/>.
        /// </summary>
        /// <param name="transaction">Transaction to display.</param>
        /// <returns>Transaction frame.</returns>
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

            // Card body
            StackLayout body = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(6, 4),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Time & Currency
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

            // Time
            Label lblTime = new Label
            {
                Text = transaction.TimeStamp.ToDateTime().ToString("HH:mm"),
                FontSize = 14,

                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            // Currency
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

            // Sender / Recipient
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

            // Set children
            BoxView spacer1 = new BoxView
            {
                BackgroundColor = SPACER_COLOR
            };
            BoxView spacer2 = new BoxView
            {
                BackgroundColor = SPACER_COLOR
            };

            left.Children.Add(lblTime, 0, 0);
            left.Children.Add(spacer1, 1, 0);
            left.Children.Add(imgCurrency, 2, 0);
            left.Children.Add(lblCurrency, 3, 0);
            left.Children.Add(spacer2, 4, 0);

            grdWallets.Children.Add(lblSender, 0, 0);
            grdWallets.Children.Add(lblRecipient, 0, 1);

            frame.Content = body;

            body.Children.Add(left);
            body.Children.Add(grdWallets);

            return frame;
        }

        /// <summary>
        /// Tap callback for the load more items button.
        /// </summary>
        private void LoadMore_Clicked(object sender, EventArgs e)
        {
            LoadMore();
        }

        /// <summary>
        /// Loads more transactions, if any are still <see cref="pending"/>.
        /// </summary>
        /// <param name="amount">Maximum amount of transactions to load.</param>
        private void LoadMore(int amount = 10)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException("Amount must be a positive integer.");

            for (int i = 0; i < amount; i++)
            {
                if (pending.Count == 0) break;
                stackTransactions.Children.Add(CreateTransactionFrame(pending.Dequeue()));
            }
        }
    }
}