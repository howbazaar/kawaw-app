using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Kawaw
{
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