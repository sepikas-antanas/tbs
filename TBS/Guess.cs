using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace TBS
{
    class Guess : INotifyPropertyChanged
    {
        private int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; OnPropertyChanged("Index"); }
        }

        private int _value = 0;
        public int Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        private int _count = 0;
        public int Count
        {
            get { return _count; }
            set { _count = value; OnPropertyChanged("Count"); }
        }

        private int _previous = 0;
        public int Previous
        {
            get { return _previous; }
            set { _previous = value; OnPropertyChanged("Previous"); }
        }

        public Guess(int index, int value, int count, int previous)
        {
            Index = index;
            Value = value;
            Count = count;
            Previous = previous;
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
