namespace Sales.Helpers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using Interfaces;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [ContentProperty("Text")]
    public class TraslateExtensions : IMarkupExtension
    {
        readonly CultureInfo ci;
        const string ResourceId = "Sales.Resources.Resource";

        static readonly Lazy<ResourceManager> ResMgr = 
            new Lazy<ResourceManager>(() => 
            new ResourceManager(ResourceId, 
                typeof(TraslateExtensions).GetTypeInfo().Assembly));

        public TraslateExtensions()
        {
            ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
        }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return "";
            }
            var translation = ResMgr.Value.GetString(Text, ci);
            if (translation == null)
            {
                throw new ArgumentException(String.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, ci.Name), "Text");
            }
            return translation;
        }

    }
}
