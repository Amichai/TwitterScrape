using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataViz;
using System.ComponentModel;

namespace Visualizer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            this.bw.RunWorkerAsync();
        }
        MediaFetcher fetcher = new MediaFetcher();
        HashSet<string> strings = new HashSet<string>();

        void bw_DoWork(object sender, DoWorkEventArgs e) {
            var db = DataStore.DataUtil.GetDataContext();
            var media = DataStore.Program.MediaElements(db);
            var ordered = media.OrderByDescending(i => i.Value);
            Dispatcher.Invoke((Action)(() => {
                this.Images.ItemsSource = null;
                this.Images.ItemsSource = ordered;
            }));
        }

        BackgroundWorker bw = new BackgroundWorker();
    }
}
