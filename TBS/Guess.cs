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
        private string _value = "";
        private int _count = 0;

        public int Index
        {
            get { return _index; }
            set { _index = value; OnPropertyChanged("Index"); }
        }

        public String Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; OnPropertyChanged("Count"); }
        }

        public Guess(int index, string value, int count)
        {
            Index = index;
            Value = value;
            Count = count;
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
