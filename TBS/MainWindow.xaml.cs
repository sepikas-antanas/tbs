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
using System.Windows.Data;

namespace TBS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker Poll;
        private BackgroundWorker StatusWorker;
        private BackgroundWorker TmpWorker;
        private Poller Poller;
        private bool _instant = true;

        public MainWindow()
        {
            InitializeComponent();

            Poller = new Poller();

            StatusWorker = new BackgroundWorker();
            StatusWorker.DoWork += StatusWorker_DoWork;
            StatusWorker.RunWorkerCompleted += StatusWorker_RunWorkerCompleted;
            StatusWorker.RunWorkerAsync();

            //poll timeout loop
            Poll = new BackgroundWorker();
            Poll.WorkerSupportsCancellation = true;
            Poll.DoWork += PollWorker_DoWork;
            Poll.RunWorkerCompleted += PollWorker_RunWorkerCompleted;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TBS_ContentRendered(object sender, EventArgs e)
        {
            /*
            if (double.Parse(DateTime.Now.ToString("yyyyMMddHHmmssffff")) > double.Parse("201501231938430619"))
            {
                MessageBox.Show("TRIAL VERSION EXPIRED!");
                Application.Current.Shutdown();
            }
            else
            {
                Poll.RunWorkerAsync();
            }
            */
            Poll.RunWorkerAsync();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            _instant = true;
        }

        private void UpdatePollTimeout_Click(object sender, RoutedEventArgs e)
        {
            PollTimeout.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            _instant = true;
        }

        private void GuessRangeUpdate_Click(object sender, RoutedEventArgs e)
        {
            TmpWorker = new BackgroundWorker();
            TmpWorker.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                GuessRange.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                DisplayRange.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                Poller.GenerateTrackList();
            };
            TmpWorker.RunWorkerCompleted += delegate(object sender2, RunWorkerCompletedEventArgs e2)
            {
                _instant = true;
            };
            TmpWorker.RunWorkerAsync();
        }

        private void UpTrack_Click(object sender, RoutedEventArgs e)
        {
            TmpWorker = new BackgroundWorker();
            TmpWorker.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                Poller.From = (Poller.From - Poller.DisplayRange < 0 ? 0 : Poller.From - Poller.DisplayRange);
                Poller.SelectPartTrackList();
            };
            TmpWorker.RunWorkerAsync();
        }

        private void DownTrack_Click(object sender, RoutedEventArgs e)
        {
            TmpWorker = new BackgroundWorker();
            TmpWorker.DoWork += delegate(object sender2, DoWorkEventArgs e2)
            {
                Poller.From = (Poller.From + Poller.DisplayRange > Poller.GuessRange ? Poller.GuessRange : Poller.From + Poller.DisplayRange);
                Poller.SelectPartTrackList();
            };
            TmpWorker.RunWorkerAsync();
        }

        private void PollWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Poll.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            Thread.Sleep(100);
            try
            {
                Poller.Process(_instant);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void PollWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _instant = false;
            //DataContext = Poller;

            if (double.Parse(DateTime.Now.ToString("yyyyMMddHHmmssffff")) > double.Parse("201501261938430619"))
            {
                var ok = MessageBox.Show("TRIAL VERSION EXPIRED!", "ALERT", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (ok == MessageBoxResult.OK) {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Poll.RunWorkerAsync();
            }
        }

        private void StatusWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
        }
        private void StatusWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataContext = Poller;
            StatusWorker.RunWorkerAsync();
        }

        private void ShowFromChange_Click(object sender, RoutedEventArgs e)
        {
            ShowFrom.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            Poller.SelectPartTrackList();
        }
    }

    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] _statusArray = 
            { 
                "Loading...", 
                "Requesting data...", 
                "Updataing data...", 
                "Generating new guess values...", 
                "Checking guess values...", 
                "Selecting data...", 
                "Generating new data tracking list...",
                "No new data..."
            };
            
            if (_statusArray.Contains(value.ToString()))
            {
                return "Visible";
            }

            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Loading...";
        }
    }

    public class MilisecondsToMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds(double.Parse(value.ToString())).TotalMinutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TimeSpan.FromMinutes(double.Parse(value.ToString())).TotalMilliseconds;
        }
    }
}
