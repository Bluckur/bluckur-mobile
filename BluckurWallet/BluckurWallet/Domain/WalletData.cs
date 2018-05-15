using System;
namespace BluckurWallet.Domain
{
    public class WalletData
    {
		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }

        public WalletData(string publicKey, string privateKey)
        {
			this.PublicKey = publicKey;
			this.PrivateKey = privateKey;
        }
    }
}
