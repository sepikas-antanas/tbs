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
        //private int[,] _trackList = new int[1000000, 2];
        //private Guess[] _trackList = new Guess[1000000];
        //private Roll[] _rollList = new Roll[10];
        private string _address = "http://topsport.betgames.tv/ext/game/results/topsport/";
        private string _tableId = "table";

        private string _status = "Loading...";
        public String PollStatus
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("PollStatus"); }
        }
        
        private int _timeout = 300000;
        public int PollTimeout
        {
            get { return _timeout; }
            set { _timeout = value; OnPropertyChanged("PollTimeout"); }
        }

        private int _elapsed = 0;
        public int PollTimeElapsed
        {
            get { return _elapsed; }
            set { _elapsed = value; OnPropertyChanged("PollTimeElapsed"); }
        }

        private int _guessRange = 1000000;
        public int GuessRange
        {
            get { return _guessRange; }
            set { _guessRange = value; OnPropertyChanged("GuessRange"); }
        }

        private int _start = 0;
        public int Start
        {
            get { return _start; }
            set { _start = value; OnPropertyChanged("Start"); }
        }

        private int _current = 100;
        public int Current
        {
            get { return _current; }
            set { _current = value; OnPropertyChanged("Current"); }
        }

        private ObservableCollection<Guess> _trackList = new ObservableCollection<Guess>();
        public ObservableCollection<Guess> TrackList
        {
            get { return _trackList; }
            set { _trackList = value; OnPropertyChanged("TrackList"); }
        }

        private ObservableCollection<Roll> _rollList = new ObservableCollection<Roll>();
        public ObservableCollection<Roll> RollList
        {
            get { return _rollList; }
            set { _rollList = value; OnPropertyChanged("RollList"); }
        }

        private Statistics _stat = new Statistics();
        public Statistics Statistics
        {
            get { return _stat; }
            set { _stat = value; OnPropertyChanged("Statistics"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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

        private void RequestData(bool first = false)
        {
            PollStatus = "Requesting data...";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(_address + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/1");
            HtmlNode table = document.GetElementbyId(_tableId).SelectSingleNode("//tbody");
            HtmlNode _td;
            
            PollStatus = "Updataing data...";
            for (int i = 0; i < 10; i++)
            {
                _td = table.SelectNodes("tr").Elements("td").Skip(1).Where(x => x.SelectSingleNode("span") != null).ToArray()[i];
                RollList[i] = new Roll(_td.SelectNodes("span").Select(s => int.Parse(s.SelectSingleNode("span").InnerText)).ToList());
            }
        }

        public void UpdateTrackList(bool _new = true)
        {
            Random rand = new Random();

            if (_new == true)
            {
                TrackList = new ObservableCollection<Guess>();
                //_trackList = new Guess[GuessRange];
                //_trackList = new int[GuessRange,2];
            }

            string[] values = {"≠", "="};

            for (int i = 0; i < 100 ; i++) //_trackList.GetLength(0)
            {
                TrackList.Add(new Guess(i + 1, values[rand.Next(1, 3) - 1], 0));
                //_trackList[i] = new Guess(i+1, values[rand.Next(1, 3) - 1], 0);
                //_trackList[i, 0] = rand.Next(1, 3);
                //_trackList[i, 1] = 0;
            }
        }

        public void IncreaseTracList(int increment = 100)
        {
            Current = Current + increment;
        }
    }
}
