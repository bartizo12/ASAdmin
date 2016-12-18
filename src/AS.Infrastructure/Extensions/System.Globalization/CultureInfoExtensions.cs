namespace System.Globalization
{
    public static class CultureInfoExtensions
    {
        public static string GetNativeNameWithoutCountryName(this CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                throw new ArgumentNullException("cultureInfo");

            if (cultureInfo.IsNeutralCulture)
                return cultureInfo.NativeName;
            return cultureInfo.Parent.NativeName;
        }
    }
}