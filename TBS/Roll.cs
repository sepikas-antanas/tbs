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
        private List<int> _hitList;
        private List<int> _missList;

        public List<int> HitList
        {
            get { return _hitList; }
            set { _hitList = value; OnPropertyChanged("HitList"); }
        }

        public List<int> MissList
        {
            get { return _missList; }
            set { _missList = value; OnPropertyChanged("MissList"); }
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

    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<int> list = value as List<int>;
            return String.Join("", list.ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string list = value as string;
            return list.Split(' ').Select(n => int.Parse(n)).ToList();
        }
    }
}
