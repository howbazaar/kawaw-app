using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class EventsView : BaseLogoutView
    {
        public EventsView()
        {
            Title = "Events";
            Content = new Label { Text = "events view" };
        }
    }
}