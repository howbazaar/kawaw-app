using System.Diagnostics;
using Xamarin.Forms;

namespace Kawaw
{
    class EventsView : BaseView
    {
        public EventsView()
        {
            Title = "Events";
            Icon = "kawaw.png";
            Content = new Label { Text = "events view" };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () =>
            {
                Debug.WriteLine("logout");
                MessagingCenter.Send<object>(this, "logout");

            }, ToolbarItemOrder.Secondary));
        }
    }
}