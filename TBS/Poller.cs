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
        private int _guessRange = 100;

        private ObservableCollection<Roll> _rollList = new ObservableCollection<Roll>();
        private ObservableCollection<Guess> _trackList = new ObservableCollection<Guess>();

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

        public ObservableCollection<Guess> TrackList
        {
            get { return _trackList; }
            set { _trackList = value; OnPropertyChanged("TrackList"); }
        }

        public int Guessrange
        {
            get { return _guessRange; }
            set { _guessRange = value; OnPropertyChanged("GuessRange"); }
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
            UpdateTrackList();
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
            
            //odd
            if (RollList.First().HitList.First() % 2 != 0)
            {
                foreach (Guess guess in TrackList)
                {
                    guess.Count = (guess.Value == 2 ? guess.Count + 1 : 0);
                }
            }
            else
            //even
            {
                foreach (Guess guess in TrackList)
                {
                    guess.Count = (guess.Value == 1 ? guess.Count + 1 : 0);
                }
            }
        }

        public void UpdateTrackList()
        {
            Random rand = new Random();
            for (int i = 1; i <= Guessrange; i++)
            {
                TrackList.Add(new Guess(i, rand.Next(1, 3), 0));
            }
        }
    }
}
