using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Kawaw.Exceptions;
using Kawaw.Framework;
using Kawaw.Models;
using Xamarin.Forms;

namespace Kawaw
{
    struct ConnectionAction
    {
        public Connection Connection;
        public string Name;
    }

    struct ConnectionActionOptions
    {
        public Connection Connection;
        public List<Tuple<string, string>> Options;
    }

    class ConnectionsViewModel : BaseViewModel
    {
        private IList<Connection> _connections;
        private Connection _selectedItem;
        private string _emptyText;

        public double EmptyOpacity { get { return Connections.Count == 0 ? 1.0 : 0.0;  } }
        public double ListOpacity { get { return Connections.Count > 0 ? 1.0 : 0.0; } }

        public string EmptyText
        {
            get { return _emptyText; }
            private set { _emptyText = value; }
        }

        public IList<Connection> Connections
        {
            get { return _connections; }
            private set { SetProperty(ref _connections, value); }
        }

        public Connection SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                var changed = SetProperty(ref _selectedItem, value);
                if (value == null || !changed) return;

                SelectedItem = null;

                var options = new ConnectionActionOptions
                {
                    Connection= value,
                    Options = new List<Tuple<string, string>>(),
                };
                if (value.Accepted || value.Pending)
                {
                    options.Options.Add(new Tuple<string, string>("reject", "Reject connection"));
                }
                if (!value.Accepted || value.Pending)
                {
                    options.Options.Add(new Tuple<string, string>("accept", "Accept connection"));
                }
                MessagingCenter.Send(this, "show-options", options);
            }
        }

        public ConnectionsViewModel(IApp app)
            : base(app, RootViewModel.Connections)
        {
            UpdateFromUser(app.User);

            MessagingCenter.Subscribe<object>(this, "connections-updated", delegate
            {
                if (IsPageVisible)
                {
                    UpdateFromUser(app.User);
                }
            });
            MessagingCenter.Subscribe(this, "connection-action", async (object sender, ConnectionAction action) =>
            {
                try
                {
                    Debug.WriteLine("{0} for {1}", action.Name, action.Connection.Name);
                    await app.User.ConnectionAction(action.Connection, action.Name == "accept");
                }
                catch (InconsistentStateException)
                {
                    MessagingCenter.Send((object)this, "refresh");
                }
                catch (Exception e)
                {
                    MessagingCenter.Send(this, "alert", new Alert
                    {
                        Title = "Update Failed",
                        Text = e.Message
                    });
                }
            });
        }

        private void UpdateFromUser(User user)
        {
            if (user == null || user.Connections == null)
            {
                // empty
                Connections = new ObservableCollection<Connection>();
                return;
            }
            Connections = new ObservableCollection<Connection>(
                from connection in user.Connections
                orderby connection.Pending descending, connection.Accepted descending, connection.Organisation, connection.Name
                select connection);

            if (user.HasVerifiedEmail)
            {
                EmptyText =
                    "No connections yet.\n\n" +
                    "As your verified email addresses are added as contact email addresses for people in " +
                    "shools, clubs, or other organisations that use kawaw, connections will show up here.";
            }
            else
            {
                EmptyText =
                    "Connections are only made using verified email addreses.\n\n" +
                    "To see any existing connections you need to verify your email " +
                    "addresses by clicking on the link in the email sent to that address.";
            }
        }

    }
}