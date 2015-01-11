using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using System.ComponentModel;
using System.Collections.ObjectModel;
using MoreLinq;

namespace TBS
{
    class Poller : INotifyPropertyChanged
    {
        private string _address = "http://topsport.betgames.tv/ext/game/results/topsport/";
        public String PollAddress
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("PollAddress"); }
        }

        private string _tableId = "table";
        public String TableId
        {
            get { return _tableId; }
            set { _tableId = value; OnPropertyChanged("TableId"); }
        }

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

        private int _displayRange = 2500;
        public int DisplayRange
        {
            get { return _displayRange; }
            set { 
                _displayRange = value;
                Statistics.PageCount = TrackList.Count / DisplayRange;
                OnPropertyChanged("DisplayRange");  
            }
        }

        private ObservableCollection<Guess> _trackList = new ObservableCollection<Guess>();
        public ObservableCollection<Guess> TrackList
        {
            get { return _trackList; }
            set { 
                _trackList = value;
                Statistics.PageCount = TrackList.Count / DisplayRange;
                OnPropertyChanged("TrackList"); 
            }
        }

        private ObservableCollection<Guess> _partTrackList = new ObservableCollection<Guess>();
        public ObservableCollection<Guess> PartTrackList
        {
            get { return _partTrackList; }
            set { _partTrackList = value; OnPropertyChanged("PartTrackList"); }
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

        private int _from = 0;
        public int From
        {
            get { return _from; }
            set { _from = value; OnPropertyChanged("From"); }
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
            GenerateTrackList();
        }

        public void Process(bool instant = false)
        {
            if (instant == true) {
                PollTimeElapsed = 0;
                RequestData();
                return;
            }
            PollStatus = "Waiting...";
            //PollTimeElapsed = PollTimeElapsed + 100;

            if ((PollTimeElapsed = PollTimeElapsed + 100) == PollTimeout)
            {
                RequestData();
                PollTimeElapsed = 0;
            }
        }

        private void RequestData()
        {
            PollStatus = "Requesting data...";
            HtmlDocument document = new HtmlWeb().Load(_address + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "/1");
            HtmlNode table = document.GetElementbyId(_tableId).SelectSingleNode("//tbody");

            if (RollList.Count() > 0) {
                HtmlNode _td = table.SelectNodes("tr").Elements("td").Skip(1).Where(x => x.SelectSingleNode("span") != null).Take(1).FirstOrDefault();
                Roll tmpRoll = new Roll(_td.SelectNodes("span").Select(s => int.Parse(s.SelectSingleNode("span").InnerText)).ToList());
                /*
                if (tmpRoll.HitList.SequenceEqual(RollList.First().HitList)) == true)
                {
                    return;
                }
                 * */
            }

            PollStatus = "Updataing data...";
            RollList = new ObservableCollection<Roll>();
            foreach (HtmlNode _td in table.SelectNodes("tr").Elements("td").Skip(1).Where(x => x.SelectSingleNode("span") != null).Take(10))
            {
                Roll roll = new Roll(_td.SelectNodes("span").Select(s => int.Parse(s.SelectSingleNode("span").InnerText)).ToList());
                App.Current.Dispatcher.Invoke((Action)delegate { RollList.Add(roll); });   
            }
            
            CheckGuessValues();
            GenerateGuessValues();
            SelectPartTrackList();
        }

        public void GenerateTrackList()
        {
            Random rand = new Random();
            TrackList = new ObservableCollection<Guess>();

            for (int i = 0; i < GuessRange; i++)
            {
                TrackList.Add(new Guess(i + 1, rand.Next(1, 3), 0));
            }
        }

        public void SelectPartTrackList()
        {
            PartTrackList = new ObservableCollection<Guess>(TrackList.Skip(From).Take(DisplayRange).OrderByDescending(g => g.Count));
        }

        private void GenerateGuessValues()
        {
            PollStatus = "Generating new guess values...";
            Random rand = new Random();
            foreach (Guess guess in TrackList)
            {
                guess.Value = rand.Next(1, 3);
            }
        }

        private void CheckGuessValues()
        {
            PollStatus = "Checking guess values...";
            //odd values
            foreach (Guess guess in TrackList.Where(g => g.Value == 1))
            {
                guess.Count = (RollList.First().HitList.First() % 2 != 0 ? 0 : guess.Count + 1);
            }
            //even values
            foreach (Guess guess in TrackList.Where(g => g.Value == 2))
            {
                guess.Count = (RollList.First().HitList.First() % 2 == 0 ? 0 : guess.Count + 1);
            }

            Statistics.RecordCount = TrackList.MaxBy(x => x.Count).Count;
        }
    }
}

/*
//if first number is odd
if (RollList.First().HitList.First() % 2 != 0)
{
    //odd values
    foreach (Guess guess in TrackList.Where(g => g.Value == 1))
    {
        guess.Count = 0;
    }
    //even values
    foreach (Guess guess in TrackList.Where(g => g.Value == 2))
    {
        guess.Count = guess.Count + 1;
    }
}
//if first number is even
else
{
    //odd values
    foreach (Guess guess in TrackList.Where(g => g.Value == 1))
    {
        guess.Count = guess.Count + 1;
    }
    //even values
    foreach (Guess guess in TrackList.Where(g => g.Value == 2))
    {
        guess.Count = 0;
    }
}
*/