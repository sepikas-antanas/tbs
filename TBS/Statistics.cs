using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace TBS
{
    class Statistics : INotifyPropertyChanged
    {
        private int _recordCount = 0;
        public int RecordCount
        {
            get { return _recordCount; }
            set { _recordCount = value; OnPropertyChanged("RecordCount"); }
        }

        private int _pageCount = 0;
        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; OnPropertyChanged("PageCount"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
