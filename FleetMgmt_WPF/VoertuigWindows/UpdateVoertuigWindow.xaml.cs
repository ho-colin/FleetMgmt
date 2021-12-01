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
    /// Interaction logic for UpdateVoertuig.xaml
    /// </summary>
    public partial class UpdateVoertuigWindow : Window {
        VoertuigManager vm = new VoertuigManager(new VoertuigRepository());
        Voertuig Voertuig { get; set; }
        Bestuurder Bestuurder = null;
        public UpdateVoertuigWindow(Voertuig v) {
            this.Voertuig = v;
            if(this.Bestuurder != null) { this.Bestuurder = Voertuig.Bestuurder; }
            InitializeComponent();
            resetVelden();
        }

        private void btn_Bestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow stw = new SelecteerBestuurderWindow();
            if(stw.ShowDialog() == true) {
                this.Bestuurder = stw.Bestuurder;
                lbl_Bestuurder.Content = this.Bestuurder.Naam;
            }
        }

        private void btn_TypeVoertuig_Click(object sender, RoutedEventArgs e) {

        }
        private void resetVelden() {
            try {
                //Nieuwe waarden kolom
                txtbx_Chassisnummer.Text = Voertuig.Chassisnummer;
                txtbx_Merk.Text = Voertuig.Merk;
                txtbx_Model.Text = Voertuig.Model;
                txtbx_Nummerplaat.Text = Voertuig.Nummerplaat;
                combobx_Brandstof.ItemsSource = Enum.GetValues(typeof(BrandstofEnum));
                if(this.Bestuurder == null) {
                    lbl_NieuwBestuurder.Content = "Geen Bestuurder";
                } else {
                    lbl_NieuwBestuurder.Content = this.Bestuurder.Naam;
                }
                if(Voertuig.TypeVoertuig == null) {
                    lbl_NieuwTypeVoertuig.Content = "Geen TypeVoertuig";
                } else {
                    lbl_NieuwTypeVoertuig.Content = Voertuig.TypeVoertuig.ToString();
                }
                txtbx_Kleur.Text = Voertuig.Kleur;
                txtbx_AantalDeuren.Text = Voertuig.AantalDeuren.ToString();

                //Huidige waarden kolom
                txtbx_HuidigChassisnummer.Text = Voertuig.Chassisnummer;
                txtbx_HuidigMerk.Text = Voertuig.Merk;
                txtbx_HuidigModel.Text = Voertuig.Model;
                txtbx_HuidigNummerplaat.Text = Voertuig.Nummerplaat;
                txtbx_HuidigBrandstof.Text = Voertuig.Brandstof.ToString();
                txtbx_HuidigTypeVoertuig.Text = Voertuig.TypeVoertuig.ToString();
                txtbx_HuidigKleur.Text = Voertuig.Kleur;
                txtbx_HuidigAantalDeuren.Text = Voertuig.AantalDeuren.ToString();
                txtbx_HuidigBestuurder.Text = Voertuig.Bestuurder.Naam;
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
