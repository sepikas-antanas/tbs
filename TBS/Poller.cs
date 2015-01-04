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
        private string _address = "http://topsport.betgames.tv/ext/game/results/topsport/";
        private string _status = "Loading...";
        private string _tableId = "table";
        private int _timeout = 300000;
        private int _elapsed = 0;
        private ObservableCollection<Roll> _rollList = new ObservableCollection<Roll>();
        private Dictionary<int, int> _trackList = new Dictionary<int, int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public String PollAddress
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("PollAddress"); }
        }

        public String TableId
        {
            get { return _tableId; }
            set { _tableId = value; OnPropertyChanged("TableId"); }
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

        public ObservableCollection<Roll> RollList
        {
            get { return _rollList; }
            set { _rollList = value; OnPropertyChanged("RollList"); }
        }

        public Dictionary<int, int> TrackList
        {
            get { return _trackList; }
            set { _trackList = value; OnPropertyChanged("TrackList"); }
        }
        
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Poller()
        {
            for (int i = 1; i <= 42; i++)
            {
                TrackList[i] = 0;
            }
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
                //update status
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

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(PollAddress + string.Join("-", date) + "/1");
            HtmlNode table = document.GetElementbyId(TableId).SelectSingleNode("//tbody");

            //update status
            PollStatus = "Parsing data...";

            foreach (HtmlNode td in table.SelectNodes("tr").Elements("td").Skip(1).Where(x => x.SelectSingleNode("span") != null))
            {
                Roll roll = new Roll(td.SelectNodes("span").Select(s => int.Parse(s.SelectSingleNode("span").InnerText)).ToList());

                PollStatus = "Updataing data...";

                if (first == true && RollList.Count > 0)
                {
                    if (!Enumerable.SequenceEqual(RollList.First().HitList, roll.HitList))
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate { RollList.Insert(0, roll); });
                    }
                    break;
                }
                RollList.Add(roll);
            }

            foreach (Roll roll in RollList.Reverse()) {
                for (int i = 1; i <= roll.MissList.Count; i++)
                {
                    TrackList[i] = (roll.MissList[i] == 1 ? TrackList[i] + 1 : 0);
                }
            }
        }
    }
}
