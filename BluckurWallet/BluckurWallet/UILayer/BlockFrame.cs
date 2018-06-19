using BluckurWallet.Domain;
using Xamarin.Forms;

namespace BluckurWallet.UILayer
{
    public class BlockFrame : Frame
    {
        public Block Block { get; set; }

        public Label LabelTransactionCount { get; set; }

        public Label LabelTransactionAmount { get; set; }
    }
}
