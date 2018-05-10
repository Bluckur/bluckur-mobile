using System;
using System.Collections.Generic;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BluckurWallet.Util
{
    public class QRCodeUtil
    {
		public static ZXingScannerPage CreateScannerPage()
        {
            ZXingScannerPage scanPage = new ZXingScannerPage();

            //setup options
            var options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.QR_CODE
                }
            };

            //add options and customize page
            scanPage = new ZXingScannerPage(options)
            {
                DefaultOverlayTopText = "Align the barcode within the frame",
                DefaultOverlayBottomText = string.Empty,
                DefaultOverlayShowFlashButton = true
            };

            return scanPage;
        }
    }
}
