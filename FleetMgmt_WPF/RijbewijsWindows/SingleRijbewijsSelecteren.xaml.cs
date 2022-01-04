using FleetMgmt_Business.Enums;
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

namespace FleetMgmt_WPF.RijbewijsWindows {
    /// <summary>
    /// Interaction logic for SingleRijbewijsSelecteren.xaml
    /// </summary>
    public partial class SingleRijbewijsSelecteren : Window {

        public RijbewijsEnum? Rijbewijs = null;

        public SingleRijbewijsSelecteren() {
            InitializeComponent();

            lstbx_Rijbewijzen.ItemsSource = Enum.GetValues(typeof(RijbewijsEnum));
        }

        private void btn_Selecteer_Click(object sender, RoutedEventArgs e) {
            this.Rijbewijs = (RijbewijsEnum) lstbx_Rijbewijzen.SelectedItem;
            DialogResult = true;
            this.Close();
        }

        private void lstbx_Rijbewijzen_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lstbx_Rijbewijzen.SelectedItem == null) {
                btn_Selecteer.IsEnabled = false;
            } else {
                btn_Selecteer.IsEnabled = true;
            }
        }
    }
}
