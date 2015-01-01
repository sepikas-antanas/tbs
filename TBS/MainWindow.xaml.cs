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

namespace TBS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PollAdress = "http://topsport.betgames.tv/ext/game/results/topsport/";
        private int PollTimeout = 5000;
        private string TableId = "table";

        private Poller Poller;

        public MainWindow()
        {
            InitializeComponent();
            
            Poller = new Poller(PollAdress, TableId, PollTimeout);
            
            this.DataContext = Poller;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TBS_ContentRendered(object sender, EventArgs e)
        {
            StatusState.Content = "Waiting...";
            Progress.Value = 100;

            

            StatusState.Content = "Waiting...";
            Progress.Value = 0;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Poller.RefreshList();
            this.DataContext = null;
            this.DataContext = Poller;
            MessageBox.Show("Updated");
        }

    }
}
