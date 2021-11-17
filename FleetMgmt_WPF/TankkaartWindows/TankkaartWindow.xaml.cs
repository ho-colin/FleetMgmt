using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FleetMgmt_WPF.TankkaartWindows {
    /// <summary>
    /// Interaction logic for TankkaartWindow.xaml
    /// </summary>
    public partial class TankkaartWindow : Window {

        TankkaartManager tm = new TankkaartManager(new TankkaartRepository());

        private FleetMgmt_Business.Objects.Bestuurder b { get; set; }

        private ObservableCollection<FleetMgmt_Business.Objects.Bestuurder> gevondenBestuurders = new ObservableCollection<FleetMgmt_Business.Objects.Bestuurder>();

        public TankkaartWindow() {
            InitializeComponent();
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            //IMPLEMENTEREN BESTUURD SELECTEREN VENSTER
        }

        private void btn_TankkaartToevoegen_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_TankkaartZoeken_Click(object sender, RoutedEventArgs e) {
            int? gevondenId = string.IsNullOrWhiteSpace(txtbw_Id.Text) ? null : int.Parse(txtbw_Id.Text);
            DateTime? gevondenDatum = txtbx_Geldigheidsdatum.SelectedDate.HasValue ? txtbx_Geldigheidsdatum.SelectedDate.Value : null;
            bool gevondenGeblokkeerd = chekbx_Geblokkeerd.IsEnabled;
            FleetMgmt_Business.Enums.TankkaartBrandstof? gevondenBrandstof = combobx_Brandstof.SelectedItem == null ? null : (FleetMgmt_Business.Enums.TankkaartBrandstof) combobx_Brandstof.SelectedItem;
            FleetMgmt_Business.Objects.Bestuurder gevondenBestuurder = this.b == null ? null : this.b;


        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void resetVelden() {
            txtbw_Id.Text = "";
            txtbx_Geldigheidsdatum.SelectedDate = DateTime.Today;
            chekbx_Geblokkeerd.IsChecked = false;
            b = null;
            lbl_BestuurderNaam.Content = "";
            gevondenBestuurders.Clear();
            lstVw_Bestuurders.ItemsSource = gevondenBestuurders;
        }
    }
}
