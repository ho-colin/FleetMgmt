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
using System.Windows.Shapes;

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for VoertuigWindow.xaml
    /// </summary>
    public partial class VoertuigWindow : Window {
        public VoertuigWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }
    }
}
