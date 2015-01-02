using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using System.ComponentModel;
using System.Collections.ObjectModel;
//using System.Timers;

namespace TBS
{
    class Poller : INotifyPropertyChanged
    {
        private string _address;
        private string _status;
        private string _table;
        private int _timeout;
        private int _elapsed;

        public String PollAdress
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("PollAddress"); }
        }

        public String TableId
        {
            get { return _table; }
            set { _table = value; OnPropertyChanged("TableId"); }
        }

        public int PollTimeout
        {
            get { return _timeout; }
            set { _timeout = value; OnPropertyChanged("PollTimeout"); }
        }

        public int PollTimeElapsed
        {
            get { return _elapsed; }
            set { _elapsed = value; OnPropertyChanged("PollTimeElapsed"); }
        }

        public String PollStatus
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("PollStatus"); }
        }

        public ObservableCollection<Roll> RollList = new ObservableCollection<Roll>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Poller(string address, string table, int timeout = 1000)
        {
            PollAdress = address;
            TableId = table;
            PollTimeout = timeout;
            PollTimeElapsed = 0;

            Process(true);
        }

        public void Process(bool instant = false, bool first = false)
        {
            if (instant == true) {
                RequestData(first);
                PollTimeElapsed = 0;
            }
            else
            {
                PollStatus = "Waiting...";
                PollTimeElapsed = PollTimeElapsed + 100;
                if (PollTimeElapsed == PollTimeout)
                {
                    RequestData(first);
                    PollTimeElapsed = 0;
                }
            }
        }

        public void Stop()
        {
            PollStatus = "Process stoped.";
        }

        private void RequestData(bool first = false)
        {
            int[] date = { DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day };

            //update status
            PollStatus = "Requesting data...";

            HtmlDocument document = new HtmlWeb().Load(PollAdress + string.Join("-", date) + "/1");
            HtmlNode table = document.GetElementbyId(TableId).SelectSingleNode("//tbody");

            //update status
            PollStatus = "Parsing data...";

            foreach (HtmlNode tr in table.SelectNodes("tr"))
            {
                foreach (HtmlNode td in tr.SelectNodes("td").Skip(1))
                {
                    if (td.SelectSingleNode("span") != null)
                    {
                        List<int> hitList = new List<int>();
                        
                        foreach (HtmlNode span in td.SelectNodes("span"))
                        {
                            hitList.Add(System.Int32.Parse(span.SelectSingleNode("span").InnerText));
                        }

                        Roll roll = new Roll() { HitList = hitList };

                        PollStatus = "Updataing data...";
                        
                        if (first == true && RollList.First() != roll)
                        {
                            RollList.Insert(0, roll);
                            break;
                        }

                        RollList.Add(roll);
                    }
                }
            }
        }
    }
}
