[assembly: Xamarin.Forms.Dependency(typeof(Sales.Droid.Implementations.Localize))]

namespace Sales.Droid.Implementations
{

    using System.Globalization;
    using System.Threading;
    using Helpers;
    using Interfaces;
   public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var androidLocale = Java.Util.Locale.Default;
            netLanguage = AndroidTodotnetLanguage(androidLocale.ToString().Replace("_", "-"));
            System.Globalization.CultureInfo ci = null;
            try
            {
                ci = new System.Globalization.CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException e1)
            {
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new System.Globalization.CultureInfo(fallback);

                }
                catch (CultureNotFoundException e2)
                {
                    ci = new System.Globalization.CultureInfo("en");
                }
            }
            return ci;
        }
        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
        string AndroidTodotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;
            switch (androidLanguage)
            {
                case "ms-BN":  //Malasian (Brunei)
                case "ms-MY":  //Malasian (Malasia)
                case "ms-SG":  //Malasian (Singapure)
                    netLanguage = "ms";
                    break;
                case "in-ID":  //Indonesian (Indonesia)
                    break;
                case "gsw-CH":  // swissGerman
                    netLanguage = "de-CH";
                    break;

            }
            return netLanguage;
        }
        string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode;
            switch (platCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH";
                    break;

            }
            return netLanguage;
        }
    }
}