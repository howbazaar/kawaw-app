using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kawaw
{
    public class UserProfile : INotifyPropertyChanged
    {
        List<string> comments;

        public UserProfile()
        {
            comments = new List<string>();
        }

        public List<string> Comments()
        {
            return comments;
        }

        public void Note(string comment)
        {
            comments.Add(comment);
            OnPropertyChanged("Data");
        }

        public IEnumerable Data
        {
            get { return comments.AsEnumerable(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
