using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
using FleetMgmt_WPF.TypeVoertuigWindows;
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
        TypeVoertuig TypeVoertuig { get; set; }
        Bestuurder Bestuurder = null;
        public UpdateVoertuigWindow(Voertuig v) {
            this.Voertuig = v;
            this.Bestuurder = Voertuig.Bestuurder;
            this.TypeVoertuig = Voertuig.TypeVoertuig;
            InitializeComponent();
            resetVelden();
        }

        private void btn_Bestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow stw = new SelecteerBestuurderWindow();
            if(stw.ShowDialog() == true) {
                this.Bestuurder = stw.bestuurder;
                lbl_NieuwBestuurder.Content = this.Bestuurder.Voornaam;
            }
        }

        private void btn_TypeVoertuig_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigSelecteren w = new TypeVoertuigSelecteren();
            if(w.ShowDialog() == true) {
                this.TypeVoertuig = w.TypeVoertuig;
                lbl_NieuwTypeVoertuig.Content = this.TypeVoertuig.Type;
            }
        }
        private void resetVelden() {
            try {
                //Nieuwe waarden kolom
                txtbx_Chassisnummer.Text = Voertuig.Chassisnummer;
                txtbx_Merk.Text = Voertuig.Merk;
                txtbx_Model.Text = Voertuig.Model;
                txtbx_Nummerplaat.Text = Voertuig.Nummerplaat;
                combobx_Brandstof.ItemsSource = Enum.GetValues(typeof(BrandstofEnum));
                combobx_Brandstof.SelectedIndex = combobx_Brandstof.Items.IndexOf(Voertuig.Brandstof);
                if(this.Bestuurder == null) {
                    lbl_NieuwBestuurder.Content = "Geen Bestuurder";
                } else {
                    lbl_NieuwBestuurder.Content = this.Bestuurder.Voornaam;
                }
                lbl_NieuwTypeVoertuig.Content = Voertuig.TypeVoertuig.Type.ToString();
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
                if (this.Bestuurder != null)
                    txtbx_HuidigBestuurder.Text = Voertuig.Bestuurder.Voornaam;
                else
                    txtbx_HuidigBestuurder.Text = "Geen Bestuurder";
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                BrandstofEnum gevondenBrandstof = (BrandstofEnum)combobx_Brandstof.SelectedItem;
                string gevondenChassisnummer = txtbx_Chassisnummer.Text;
                string gevondenKleur = string.IsNullOrWhiteSpace(txtbx_Kleur.Text) ? null : txtbx_Kleur.Text;
                int? gevondenAantalDeuren = txtbx_AantalDeuren.Text == null ? null : int.Parse(txtbx_AantalDeuren.Text);
                string gevondenMerk = string.IsNullOrWhiteSpace(txtbx_Merk.Text) ? null : txtbx_Merk.Text;
                string gevondenModel = string.IsNullOrWhiteSpace(txtbx_Model.Text) ? null : txtbx_Model.Text;
                TypeVoertuig gevondenTypeVoertuig = this.TypeVoertuig;
                string gevondenNummerplaat = string.IsNullOrWhiteSpace(txtbx_Nummerplaat.Text) ? null : txtbx_Nummerplaat.Text;
                Bestuurder gevondenBestuurder = this.Bestuurder;
                Voertuig geupdateVoertuig = new Voertuig(gevondenBrandstof, gevondenChassisnummer, gevondenKleur, gevondenAantalDeuren, gevondenMerk, gevondenModel, gevondenTypeVoertuig, gevondenNummerplaat,gevondenBestuurder);
                vm.updateVoertuig(geupdateVoertuig);
                DialogResult = true;
                Close();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }
    }
}
