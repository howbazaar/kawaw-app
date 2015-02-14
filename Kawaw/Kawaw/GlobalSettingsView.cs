using Kawaw.Framework;
using Xamarin.Forms;

namespace Kawaw
{
    class GlobalSettingsView : BaseView
    {
        public GlobalSettingsView()
        {
            Title = "Global Settings";
            Icon = "kawaw.png";
            Padding = new Thickness(20);

            var site = new Entry
            {
                Placeholder = "remote site"
            };
            site.SetBinding(Entry.TextProperty, "Site");
            Content = new StackLayout
            {
                Spacing = 10,
                Children = {
                    new Label{ Text = "Kawaw Site"},
                    site,
                }
            };
        }
    }
}