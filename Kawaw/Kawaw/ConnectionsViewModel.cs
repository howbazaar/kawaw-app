using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    Debug.WriteLine("connection selected {0}", value.Id);
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
        }

        public ConnectionsViewModel(IApp app)
            : base(app, RootViewModel.Connections)
        {
            UpdateFromUser(app.User);

            MessagingCenter.Subscribe<object>(this, "connections-updated", delegate
            {
                UpdateFromUser(app.User);
            });
            MessagingCenter.Subscribe<object, ConnectionAction>(this, "connection-action", async (object sender, ConnectionAction action) =>
            {
                try
                {
                    Debug.WriteLine("{0} for {1}", action.Name, action.Connection.Name);
                    app.User.ConnectionAction(action.Connection, action.Name == "accept");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Oops {0}", e.Message);
                }
            });

        }

        private void UpdateFromUser(RemoteUser user)
        {
            if (user == null) return;
            if (user.Connections == null)
            {
                // empty
                Connections = new ObservableCollection<Connection>();
                return;
            }
            Connections = new ObservableCollection<Connection>(
                from connection in user.Connections
                orderby connection.Pending descending, connection.Accepted descending, connection.Organisation, connection.Name
                select connection);
        }

    }
}