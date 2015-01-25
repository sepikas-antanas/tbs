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

        private Guess _record;
        public Guess Record
        {
            get { return _record; }
            set { _record = value; OnPropertyChanged("Record"); }
        }

        private int _oddCount = 0;
        public int OddCount
        {
            get { return _oddCount; }
            set { _oddCount = value; OnPropertyChanged("OddCount"); }
        }

        private int _evenCount = 0;
        public int EvenCount
        {
            get { return _evenCount; }
            set { _evenCount = value; OnPropertyChanged("EvenCount"); }
        }

        private int _oddEvenRecord = 0;
        public int OddEvenRecord
        {
            get { return _oddEvenRecord;  }
            set { _oddEvenRecord = value; OnPropertyChanged("OddEvenRecord"); }
        }

        private int _next = 0;
        public int Next
        {
            get { return _next; }
            set { _next = value; OnPropertyChanged("Next"); }
        }

        private int _oddCount18 = 0;
        public int OddCount18
        {
            get { return _oddCount18; }
            set { _oddCount18 = value; OnPropertyChanged("OddCount18"); }
        }

        private int _evenCount18 = 0;
        public int EvenCount18
        {
            get { return _evenCount18; }
            set { _evenCount18 = value; OnPropertyChanged("EvenCount18"); }
        }

        private int _oddEvenRecord18 = 0;
        public int OddEvenRecord18
        {
            get { return _oddEvenRecord18; }
            set { _oddEvenRecord18 = value; OnPropertyChanged("OddEvenRecord18"); }
        }

        private int _next18 = 0;
        public int Next18
        {
            get { return _next18; }
            set { _next18 = value; OnPropertyChanged("Next18"); }
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
