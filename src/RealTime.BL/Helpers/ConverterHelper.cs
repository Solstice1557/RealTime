namespace RealTime.BL
{
    using System.Globalization;

    public static class ConverterHelper
    {
        public static decimal? ToDecimal(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
            {
                return val;
            }

            return null;
        }
    }
}
