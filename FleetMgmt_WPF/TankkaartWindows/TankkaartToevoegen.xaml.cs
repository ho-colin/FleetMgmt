using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
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
using FleetMgmt_WPF.BestuurderWindows;

namespace FleetMgmt_WPF.TankkaartWindows {
    /// <summary>
    /// Interaction logic for TankkaartToevoegen.xaml
    /// </summary>
    public partial class TankkaartToevoegen : Window {

        List<FleetMgmt_Business.Objects.Tankkaart> tankkaarten = new List<FleetMgmt_Business.Objects.Tankkaart>();
        List<TankkaartBrandstof> brandstoffen = new List<TankkaartBrandstof>(); 

        FleetMgmt_Business.Objects.Bestuurder bestuurder;

        TankkaartManager tm = new TankkaartManager(new TankkaartRepository());

        public TankkaartToevoegen() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow w = new SelecteerBestuurderWindow();
            if(w.ShowDialog() == true) {
                this.bestuurder = w.bestuurder;
                lbl_GeselecteerdeBestuurder.Content = this.bestuurder.Voornaam;
            }
        }

        private void btn_TankkaartToevoegen_Click(object sender, RoutedEventArgs e) {
            if (txtbx_Geldigheidsdatum.SelectedDate == null) { MessageBox.Show("Gelieve een Geldigheidsdatum in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            //Checkbox is either true or false.
            if (brandstoffen.Count < 1) { MessageBox.Show("Gelieve een Brandstof te selecteren!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }

            try {
                FleetMgmt_Business.Objects.Bestuurder gevondenBestuurder = bestuurder == null ? null : bestuurder;
                string gevondenPincode = string.IsNullOrWhiteSpace(txtbw_Pincode.Text) ? null : txtbw_Pincode.Text;

                FleetMgmt_Business.Objects.Tankkaart tk = new FleetMgmt_Business.Objects.Tankkaart(txtbx_Geldigheidsdatum.SelectedDate.Value, gevondenPincode, gevondenBestuurder, brandstoffen, chekbx_Geblokkeerd.IsChecked.Value);

                FleetMgmt_Business.Objects.Tankkaart tkNew = tm.voegTankkaartToe(tk);
                tankkaarten.Add(tkNew);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

            lstVw_Tankkaarten.ItemsSource = tankkaarten;
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void txtbw_Pincode_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^\d+$")) {
                e.Handled = true;
            }
        }

        private void resetVelden() {
            txtbx_Geldigheidsdatum.SelectedDate = null;
            chekbx_Geblokkeerd.IsChecked = false;
            brandstoffen.Clear();
            txtbw_Pincode.Text = "";
            this.bestuurder = null;
            lbl_GeselecteerdeBestuurder.Content = "";
            this.brandstoffen = null;
            lbl_BrandstofAantal.Content = "";
        }

        private void btn_Brandstof_Click(object sender, RoutedEventArgs e) {
            BrandstofSelecteren w = new BrandstofSelecteren();
            if(w.ShowDialog() == true) {
                this.brandstoffen = w.brandstoffen;
                lbl_BrandstofAantal.Content = this.brandstoffen.Count;
            }
        }
    }
}
