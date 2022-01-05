using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
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
    /// Interaction logic for ToevoegenRijbewijsWindow.xaml
    /// </summary>
    public partial class ToevoegenRijbewijsWindow : Window {
        private Bestuurder Bestuurder = null;

        public ToevoegenRijbewijsWindow() {
            InitializeComponent();
            cmbbx_Rijbewijs.ItemsSource = Enum.GetValues(typeof(RijbewijsEnum));
            lbl_GeselecteerdeBestuurder.Content = Bestuurder.Voornaam;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) {
            cmbbx_Rijbewijs.SelectedItem = null;
            dtpckr_BehaaldOp.SelectedDate = null;
        }
    }
}
