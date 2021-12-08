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
    /// Interaction logic for RijbewijsSelecteren.xaml
    /// </summary>
    public partial class RijbewijsSelecteren : Window {

        public List<RijbewijsEnum> Rijbewijzen { get; set; }


        public RijbewijsSelecteren() {
            InitializeComponent();

            lstbx_Rijbewijzen.ItemsSource = Enum.GetValues(typeof(RijbewijsEnum));
        }

        private void btn_Selecteer_Click(object sender, RoutedEventArgs e) {
            if (Rijbewijzen.Count < 1)
                MessageBox.Show("Lijst is leeg!", "Selecteer rijbewijs", MessageBoxButton.OK);
            else 
                this.Rijbewijzen = lstbx_Rijbewijzen.SelectedItems.Cast<RijbewijsEnum>().ToList();
            DialogResult = true;
            this.Close();
        }
    }
}
