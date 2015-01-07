using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
/*
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
*/
using System.Windows.Controls.Ribbon;
using HtmlAgilityPack;
using System.Threading;
using System.ComponentModel;

namespace TBS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker Poll;
        private BackgroundWorker TmpWorker;
        private Poller Poller = new Poller();

        public MainWindow()
        {
            InitializeComponent();

            Poll = new BackgroundWorker();
            Poll.WorkerSupportsCancellation = true;
            
            Poll.DoWork += delegate(object sender, DoWorkEventArgs e) 
            {
                if (Poll.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(100);
                Poller.Process(false, true);
            };

            Poll.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) 
            {
                //RollList.ItemsSource = Poller.GetRollList();
                //TrackList.ItemsSource = Poller.GetTrackList();
                DataContext = Poller;
                Poll.RunWorkerAsync();
            };
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TBS_ContentRendered(object sender, EventArgs e)
        {
            if (double.Parse(DateTime.Now.ToString("yyyyMMddHHmmssffff")) > double.Parse("201501092055118697"))
            {
                MessageBox.Show("TRIAL VERSION EXPIRED!");
                Application.Current.Shutdown();
            }
            else
            {
                Poll.RunWorkerAsync();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Poller.Process(true, true);
            DataContext = Poller;
        }

        private void UpdatePollTimeout_Click(object sender, RoutedEventArgs e)
        {
            TmpWorker = new BackgroundWorker();
            TmpWorker.DoWork += delegate(object sender2, DoWorkEventArgs e2) 
            {
                PollTimeout.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            };
            TmpWorker.RunWorkerAsync();
        }

        private void GuessRangeUpdate_Click(object sender, RoutedEventArgs e)
        {

            TmpWorker = new BackgroundWorker();
            TmpWorker.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                GuessRange.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Poller.UpdateTrackList();

            };
            TmpWorker.RunWorkerAsync();
        }

        private void TrackListScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (TrackListScroll.VerticalOffset == TrackListScroll.ScrollableHeight)
            {
                //increase limtit
            }
        }
    }









    /**
     * STATISTICS CLASS 
     */
    class Statistics : INotifyPropertyChanged
    {
        private int _recordValue = 0;
        private int _recordCount = 0;
        private int _recordEventCount = 0;
        private int _recordOddCount = 0;
        //private int _count = 0;

        public int RecordValue
        {
            get { return _recordValue; }
            set { _recordValue = value; OnPropertyChanged("RecordValue"); }
        }

        public int RecordCount
        {
            get { return _recordCount; }
            set { _recordCount = value; OnPropertyChanged("RecordCount"); }
        }

        public int RecordEvenCount
        {
            get { return _recordEventCount; }
            set { _recordEventCount = value; OnPropertyChanged("RecordEvenCount"); }
        }

        public int RecordOddCount
        {
            get { return _recordOddCount; }
            set { _recordOddCount = value; OnPropertyChanged("RecordOddCount"); }
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
