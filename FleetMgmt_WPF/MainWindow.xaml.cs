using FleetMgmt_WPF.TankkaartWindows;
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

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow vw = new VoertuigWindow();
            vw.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tvw = new TypeVoertuigWindow();
            tvw.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            TankkaartWindow w = new TankkaartWindow();
            w.Show();
            this.Close();
        }
    }
}
