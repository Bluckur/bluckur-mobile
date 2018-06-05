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
        }

        /// <summary>
        /// Removes all blocks.
        /// </summary>
        private void Clear()
        {
            stackBlocks.Children.Clear();
        }

        /// <summary>
        /// Shows the search menu.
        /// </summary>
        private void Search_Clicked(object sender, EventArgs e)
        {
            // TODO: Search UI with options (date / wallet / etc). See BlockchainExplorer class.
            datePicker.Focus();
        }

        /// <summary>
        /// <see cref="DatePicker.DateSelected"/> for <see cref="datePicker"/>.
        /// </summary>
        private void DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime time = datePicker.Date;
            time = new DateTime(time.Year, time.Month, time.Day);

            stackBlocks.Children.Clear();
            SelectDay(time);
        }

        /// <summary>
        /// Shows all blocks for the selected day.
        /// </summary>
        /// <param name="day"></param>
        private async void SelectDay(DateTime day)
        {
            try
            {
                LinkedList<Block> blocks = await explorer.GetBlocksAsync(day);
                ShowBlocks(blocks);
            }
            catch (RestException exc)
            {
                await DisplayAlert("Error", exc.InnerException.Message, "Ok");
            }
        }

        /// <summary>
        /// Shows the given blocks as cards.
        /// Blocks with or without transactions are supported.
        /// Cards can be selected to show block details in a <see cref="BlockPage"/>.
        /// </summary>
        /// <param name="blocks">Blocks to show.</param>
        private void ShowBlocks(ICollection<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                BlockFrame frame = CreateBlockFrame(block);
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    NumberOfTapsRequired = 2,
                    CommandParameter = frame,
                    Command = new Command(UpdateBlockFrame)
                });
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    NumberOfTapsRequired = 1,
                    CommandParameter = frame,
                    Command = new Command(ShowBlock_Clicked)
                });

                stackBlocks.Children.Add(frame);
            }
        }

        /// <summary>
        /// Updates the data of a <see cref="BlockFrame"/>.
        /// The transaction count and amount are updated (as this data may have been unavailable initially).
        /// </summary>
        /// <param name="sender"><see cref="BlockFrame"/> to update.</param>
        private async void UpdateBlockFrame(object sender)
        {
            BlockFrame senderFrame = sender as BlockFrame;
            senderFrame.Block = await explorer.GetBlock(senderFrame.Block.Header.Hash);
            
            senderFrame.LabelTransactionCount.Text = senderFrame.Block.Transactions.Count.ToString();
            float amount = 0f;
            foreach (var item in senderFrame.Block.Transactions.Where(t => t.Type == "COIN"))
            {
                amount += item.Amount;
            }
            senderFrame.LabelTransactionAmount.Text = amount.ToString();
        }

        /// <summary>
        /// Shows a specific block and its transactions in a new page.
        /// </summary>
        /// <param name="sender">Selected <see cref="BlockFrame"/>.</param>
        private async void ShowBlock_Clicked(object sender)
        {
            BlockFrame senderFrame = sender as BlockFrame;

            if (senderFrame?.Block != null)
            {
                var page = new BlockPage(senderFrame.Block);
                await Navigation.PushAsync(page);
            }
            else
            {
                await DisplayAlert("Error", "The selected block has no data.", "OK");
            }
        }

        /// <summary>
        /// Creates a frame view displaying the details of a block.
        /// The returned value can be added to the <see cref="stackBlocks"/>.
        /// </summary>
        /// <param name="block">Block to display.</param>
        /// <returns>Block frame.</returns>
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

            // Assign block to frame
            frame.Block = block;

            // Card content
            StackLayout body = new StackLayout
            {
                Padding = new Thickness(6, 0),
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
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 40 }
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
                Text = blockTime.ToString("HH:mm"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Margin = 0,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            top.Children.Add(lblBlockTime, 1, 0);
            
            Label lblBlockDate = new Label
            {
                Text = blockTime.ToString("dd-MM-yyyy"),
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
                BackgroundColor = Color.FromRgb(235, 235, 235),
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

            frame.LabelTransactionAmount = lblTransactionAmount;
            frame.LabelTransactionCount = lblTransactions;

            return frame;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task LoadMore_Clicked(object sender, EventArgs e)
        {

        }
    }
}