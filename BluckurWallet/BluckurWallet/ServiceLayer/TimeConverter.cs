using System;

namespace BluckurWallet.ServiceLayer
{
    public static class TimeConverter
    {
        private static DateTime epoch = new DateTime(1970, 1, 1);

        public static long ToUnix(this DateTime time)
        {
            var diff = time - epoch;
            return Convert.ToInt64(diff.TotalSeconds);
        }

        public static DateTime ToDateTime(this long unix)
        {
            return epoch.AddSeconds(unix);
        }
    }
}
