using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrtsBurtgel
{
    public class UserDataItem : INotifyPropertyChanged
    {
        private string name;
        private string status;
        public int statusId;

        public event PropertyChangedEventHandler PropertyChanged;
        public string Name
        {
            get { return name; }
            set
            {
                if (!value.Equals(name, StringComparison.InvariantCulture))
                {
                    OnPropertyChanged("Name");
                    name = value;
                }
            }

        }

        public string Status
        {
            get { return status; }
            set
            {
                if (!value.Equals(name, StringComparison.InvariantCulture))
                {
                    OnPropertyChanged("Status");
                    status = value;
                }
            }
        }

        public string StatusID
        {
            get { return statusId.ToString(); }
            set
            {
                if (!value.Equals(name, StringComparison.InvariantCulture))
                {
                    OnPropertyChanged("StatusID");
                    statusId = Int32.Parse(value);
                }
            }

        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
