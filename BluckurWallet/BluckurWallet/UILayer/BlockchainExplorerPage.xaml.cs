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
            Clear();

            explorer = new BlockchainExplorer();

            #region TEST CODE
            var testblocks = new List<Block> {
                new Block()
                {
                    Header = new BlockHeader
                    {
                        BlockNumber = 1,
                        Hash = "12345",
                        PreviousHash = "abcde",
                        TimeStamp = DateTime.Now.ToUnix()
                    }
                },
                new Block()
                {
                    Header = new BlockHeader
                    {
                        BlockNumber = 1,
                        Hash = "12345",
                        PreviousHash = "abcde",
                        TimeStamp = DateTime.Now.AddMinutes(10).ToUnix()
                    },
                    Transactions =
                    {
                        new Transaction
                        {
                            Sender = "Jeroen",
                            Recipient = "Andere Jeroen",
                            Amount = 1.5f,
                            TimeStamp = DateTime.Now.AddDays(-1).ToUnix(),
                            Type = "COIN"
                        },
                        new Transaction
                        {
                            Sender = "Andere Jeroen",
                            Recipient = "Jeroen",
                            Amount = 1.75f,
                            TimeStamp = DateTime.Now.AddDays(-2).ToUnix(),
                            Type = "COIN"
                        }
                    }
                }
            };

            for (int i = 0; i < 2; i++)
            {
                testblocks.Add(testblocks[i]);
            }
            testblocks[0].Transactions = null;
            ShowBlocks(testblocks);
            #endregion
        }

        private void Clear()
        {
            stackBlocks.Children.Clear();
        }

        private void Search_Clicked(object sender, EventArgs e)
        {
            // TODO: Search UI with options (date / wallet / etc). See BlockchainExplorer class.
            datePicker.Focus();
        }

        private void DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime time = datePicker.Date;
            time = new DateTime(time.Year, time.Month, time.Day);

            stackBlocks.Children.Clear();
            SelectDay(time);
        }

        private async void SelectDay(DateTime day)
        {
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
            foreach (Block block in blocks)
            {
                BlockFrame frame = CreateBlockFrame(block);
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(ShowBlock_Clicked)
                });

                stackBlocks.Children.Add(frame);
            }
        }

        private async void ShowBlock_Clicked(object sender)
        {
            BlockFrame senderFrame = sender as BlockFrame;
            await this.DisplayAlert("Nope", "Not implemented.", "Ok");
            // await this.Navigation.PushAsync(new BlockPage(block));
        }

        private BlockFrame CreateBlockFrame(Block block)
        {
            Thickness tZero = new Thickness(0d);

            // Card
            BlockFrame frame = new BlockFrame
            {
                CornerRadius = 5,
                Padding = tZero,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            // Card content
            StackLayout body = new StackLayout
            {
                Padding = tZero,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Block number & Time
            Grid top = new Grid
            {
                Margin = new Thickness(10, 10),
                ColumnSpacing = 0,
                RowSpacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 40 },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = 40 },
                    new RowDefinition { Height = 20 }
                }
            };

            // Block image.
            Image imgBlock = new Image
            {
                Source = "ic_block.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            top.Children.Add(imgBlock, 0, 0);

            // Block number
            Label lblBlock = new Label
            {
                Text = "#" + Convert.ToString(block.Header.BlockNumber),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            top.Children.Add(lblBlock, 0, 1);

            DateTime blockTime = block.Header.TimeStamp.ToDateTime();

            // Block time
            Label lblBlockTime = new Label
            {
                Text = blockTime.ToString("dd-MM-yyyy"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = 0,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            top.Children.Add(lblBlockTime, 1, 0);

            Label lblBlockDate = new Label
            {
                Text = blockTime.ToString("HH:mm"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = 0,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            };
            top.Children.Add(lblBlockDate, 1, 1);

            // Hash
            Label hash = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 25,
                BackgroundColor = Color.FromRgb(211, 211, 211),
                Text = block.Header.Hash
            };

            // Transactions
            Grid bottom = new Grid
            {
                Margin = new Thickness(5d),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 20 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = 32 }
                }
            };

            // Transaction count
            Label lblTransactions = new Label
            {
                Text = block.Transactions?.Count.ToString() ?? "?",
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            };
            bottom.Children.Add(lblTransactions, 0, 0);

            Image imgTransactions = new Image
            {
                Source = "ic_transaction.png",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            bottom.Children.Add(imgTransactions, 1, 0);

            float? amount = null;
            if (block.Transactions != null)
            {
                amount = 0;
                foreach (var item in block.Transactions.Where(t => t.Type == "COIN"))
                {
                    amount += item.Amount;
                }
            }

            // Transaction amount
            Label lblTransactionAmount = new Label
            {
                Text = amount.HasValue ? amount.Value.ToString() : "?",
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            };
            bottom.Children.Add(lblTransactionAmount, 3, 0);

            Image imgTransactionAmount = new Image
            {
                Source = "icon.png",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            bottom.Children.Add(imgTransactionAmount, 4, 0);

            // Append to frame
            frame.Content = body;
            body.Children.Add(top);
            body.Children.Add(hash);
            body.Children.Add(bottom);

            return frame;
        }
    }
}