using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private Poller Poller;

        public MainWindow()
        {
            InitializeComponent();

            Poll = new BackgroundWorker();
            Poll.WorkerSupportsCancellation = true;
            Poll.DoWork += Poll_DoWork;
            Poll.RunWorkerCompleted += Poll_RunWorkerCompleted;

            Poller = new Poller();
            DataContext = Poller;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TBS_ContentRendered(object sender, EventArgs e)
        {
            Poll.RunWorkerAsync();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Poller.Process(true, true);
            DataContext = Poller;
        }

        private void Poll_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (Poll.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            Thread.Sleep(100);
            Poller.Process(false, true);
        }

        private void Poll_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            DataContext = Poller;
            Poll.RunWorkerAsync();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //stop polling
            //destroy Polller
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdatePollTimeout_Click(object sender, RoutedEventArgs e)
        {
            //call update login
        }
    }
}
