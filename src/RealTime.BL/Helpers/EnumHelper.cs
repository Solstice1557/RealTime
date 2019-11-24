namespace RealTime.BL
{
    using System.ComponentModel;

    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue, string defDesc) where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return defDesc;
            }

            var fi = type.GetField(enumValue.ToString());
            if (fi == null)
            {
                return defDesc;
            }

            var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }
        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            return GetDescription(enumValue, string.Empty);
        }
    }
}
