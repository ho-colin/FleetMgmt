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

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for BestuurderToevoegenWindow.xaml
    /// </summary>
    public partial class BestuurderToevoegenWindow : Window {
        public BestuurderToevoegenWindow() {
            InitializeComponent();
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            reset();
        }

        private void reset() {
            this.txtbx_Voornaam.Text = "";
            this.txtbx_AchterNaam.Text = "";
            this.txtbx_Geldigheidsdatum.Text = "";
            this.txtbx_Rijksregisiternummer.Text = "";
        }

        private void btn_BestuurderToevoegen_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_KeerTerug_Click(object sender, RoutedEventArgs e) {

        }
    }
}
