using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
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
using System.Windows.Shapes;

namespace FleetMgmt_WPF.RijbewijsWindows {
    /// <summary>
    /// Interaction logic for RijbewijsWindow.xaml
    /// </summary>
    public partial class RijbewijsWindow : Window {

        private Bestuurder Bestuurder = null;

        public RijbewijsWindow() {
            InitializeComponent();
        }

        private void btn_RijbewijsToevoegen_Click(object sender, RoutedEventArgs e) {
            ToevoegenRijbewijsWindow tv = new ToevoegenRijbewijsWindow();
            tv.Show();
            this.Close();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void Reset() {
            lblBestuurderRijbewijs.Content = "";
            lstVw_Rijbewijzen.ItemsSource = null;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            //Rijbewijs verwijderen!
        }

        private void btn_BestuurderSelecteren_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow selecteerBestuurderWindow = new SelecteerBestuurderWindow();
            if(selecteerBestuurderWindow.ShowDialog() == true) {
                this.Bestuurder = selecteerBestuurderWindow.bestuurder;
                lblBestuurderRijbewijs.Content = this.Bestuurder.Voornaam;
            }
           
        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow voertuigWindow = new VoertuigWindow();
            voertuigWindow.Show();
            this.Close();
        }

        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bestuurderWindow = new BestuurderWindow();
            bestuurderWindow.Show();
            this.Close();
        }

        private void btn_TankkaartNavigatie_Click(object sender, RoutedEventArgs e) {
            TankkaartWindow tankkaartWindow = new TankkaartWindow();
            tankkaartWindow.Show();
            this.Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow typeVoertuigWindow = new TypeVoertuigWindow();
            typeVoertuigWindow.Show();
            this.Close();
        }

        private void btn_RijbewijsNavgiatie_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("U bevindt zich in dit venster", "Rijbewijs", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
