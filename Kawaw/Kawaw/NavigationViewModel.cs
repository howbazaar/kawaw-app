using System.Collections;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Kawaw
{
    class NavigationViewModel : BaseViewModel
    {
        private NavigationItem _selectedItem;

        public class NavigationItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public NavigationItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    MessagingCenter.Send(this, "show-page", _selectedItem.Name);
                    SelectedItem = null;
                }
            }
        }

        public IList Items { get; private set; }
        public NavigationViewModel(IApp app) : base(app)
        {
            Items = new ObservableCollection<object>
            {
                new NavigationItem{Name="Events", Description="Your upcoming events"},
                new NavigationItem{Name="Connections", Description="Your connections to schools, clubs or businesses"},
                new NavigationItem{Name="Profile", Description="View or change your details"},
            };

        }
    }
}