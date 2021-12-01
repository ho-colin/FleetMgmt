using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
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

namespace FleetMgmt_WPF.VoertuigWindows {
    /// <summary>
    /// Interaction logic for VoertuigToevoegen.xaml
    /// </summary>
    public partial class VoertuigToevoegenWindow : Window {
        VoertuigManager vm = new VoertuigManager(new VoertuigRepository());
        List<Voertuig> voertuigen = new List<Voertuig>();
        Voertuig voertuig;
        Bestuurder bestuurder;
        TypeVoertuig typeVoertuig;
        public VoertuigToevoegenWindow() {
            InitializeComponent();
            combobx_Brandstof.ItemsSource = Enum.GetValues(typeof(BrandstofEnum));
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow w = new SelecteerBestuurderWindow();
            w.Show();
            this.Close();
        }

        private void btn_SelecteerTypeVoertuig_Click(object sender, RoutedEventArgs e) {
            SelecteerTypeVoertuigWindow w = new SelecteerTypeVoertuigWindow();
            if(w.ShowDialog()== true) {
                this.typeVoertuig = w.typeVoertuig;
                lbl_TypeVoertuig.Content = voertuig.TypeVoertuig;
            }
        }

        private void btn_VoertuigToevoegen_Click(object sender, RoutedEventArgs e) {
            if(txtbx_Chassisnummer.Text == null) { MessageBox.Show("Gelieve een chassisnummer in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);return; }
            if (txtbx_Merk.Text == null) { MessageBox.Show("Gelieve een merk in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (txtbx_Model.Text == null) { MessageBox.Show("Gelieve een model in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (txtbx_Nummerplaat.Text == null) { MessageBox.Show("Gelieve een nummerplaat in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (lbl_GeselecteerdeTypeVoertuig == null) { MessageBox.Show("Gelieve een chassisnummer in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if(combobx_Brandstof.SelectedItem == null) { MessageBox.Show("Gelieve een brandstof in te vullen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);return; }

            try {
                Voertuig gevondenVoertuig = voertuig == null ? null : voertuig;
                string gevondenKleur = string.IsNullOrWhiteSpace(txtbx_Kleur.Text) ? null : txtbx_Kleur.Text;
                string gevondenAantalDeuren = string.IsNullOrWhiteSpace(txtbx_AantalDeuren.Text) ? null : txtbx_AantalDeuren.Text;
                Bestuurder gevondenBestuurder = bestuurder == null ? null : bestuurder;

                voertuig = new Voertuig((BrandstofEnum)combobx_Brandstof.SelectedItem, txtbx_Chassisnummer.Text, gevondenKleur, int.Parse(gevondenAantalDeuren),
                    txtbx_Merk.Text, txtbx_Model.Text, typeVoertuig, txtbx_Nummerplaat.Text);
                Voertuig voertuigNew = vm.voegVoertuigToe(voertuig);
            } catch (Exception ex) {

                throw;
            }
        }
    }
}
