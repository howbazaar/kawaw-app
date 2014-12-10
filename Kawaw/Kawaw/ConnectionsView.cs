using Xamarin.Forms;

namespace Kawaw
{
    class ConnectionsView : BaseView
    {
        public ConnectionsView()
        {
            Title = "Connections";
            Icon = "kawaw.png";
            Content = new Label { Text = "connections view" };

            ToolbarItems.Add(new ToolbarItem("Logout", null, () => MessagingCenter.Send<object>(this, "logout"), ToolbarItemOrder.Secondary));
        }
    }
}