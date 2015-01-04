using System;
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
        //private List<int> _missList = new List<int>(new int[42]);
        private Dictionary<int, int> _missList = new Dictionary<int, int>();
        private List<int> _hitList;

        public List<int> HitList
        {
            get { return _hitList; }
            set { _hitList = value; OnPropertyChanged("HitList"); }
        }

        public Dictionary<int, int> MissList
        {
            get { return _missList; }
            set { _missList = value; OnPropertyChanged("MissList"); }
        }

        public Roll(List<int> roll)
        {
            HitList = roll;

            for (int i = 1; i <= 42; i++)
            {
                MissList[i] = roll.Contains(i) ? 0 : 1;
            }
        }

        //dar turetu buti constructorius kur paduodi lsita ir jis paskaiciuoja misslista

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
