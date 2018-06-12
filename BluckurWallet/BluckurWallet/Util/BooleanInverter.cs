using System;
using Xamarin.Forms;

namespace BluckurWallet.Util
{
	/// <summary>
    /// Is used for inverting a boolean value in the front end.
    /// </summary>
	public class BooleanInverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
