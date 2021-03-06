﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace TBS
{
    class Roll : INotifyPropertyChanged
    {
        private List<int> _hitList;
        public List<int> HitList
        {
            get { return _hitList; }
            set { _hitList = value; OnPropertyChanged("HitList"); }
        }

        public Roll(List<int> roll)
        {
            HitList = roll;
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
