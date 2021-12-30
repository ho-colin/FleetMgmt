using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.RijbewijsWindows;
using FleetMgmt_WPF.TankkaartWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for UpdateBestuurderWindow.xaml
    /// </summary>
    public partial class UpdateBestuurderWindow : Window {

        private BestuurderManager _bestuurderManager = new BestuurderManager(new BestuurderRepository());
        public Tankkaart tankkaart { get; set; }
        public List<RijbewijsEnum> rijbewijzen { get; set; }
        public Bestuurder bestuurder { get; set; }

        public UpdateBestuurderWindow(Bestuurder bestuurder) {
            this.bestuurder = bestuurder;
            InitializeComponent();
            Reset();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                string gevondenRijks = string.IsNullOrWhiteSpace(txtbx_RijksregisterNummer.Text) ? null : txtbx_RijksregisterNummer.Text;
                string gevondenNaam = string.IsNullOrWhiteSpace(txtbx_Voornaam.Text) ? null : txtbx_Voornaam.Text;
                string gevondenAchternaam = string.IsNullOrWhiteSpace(txtbx_Achternaam.Text) ? null : txtbx_Achternaam.Text;
                DateTime geboortedatum =
                    Convert.ToDateTime(DatePckr_Geboortedatum.SelectedDate.HasValue ?
                    DatePckr_Geboortedatum.SelectedDate.Value : null);
                Bestuurder b = new Bestuurder(gevondenRijks, gevondenAchternaam, gevondenNaam, geboortedatum);
                _bestuurderManager.bewerkBestuurder(b);
                DialogResult = true;
                Close();
            }catch(Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void txtbx_Voornaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingVoornaam();
        }

        private void txtbx_Achternaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingAchternaam();
        }

        private void AutoCapitalCasingVoornaam() {
            string oldText = "";
            if ((txtbx_Voornaam.SelectionStart <= txtbx_Voornaam.Text.Length - oldText.Length
                || txtbx_Voornaam.SelectionStart == 0) &&
                char.IsLower(txtbx_Voornaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Voornaam.SelectionStart;
                var selectionLength = txtbx_Voornaam.SelectionLength;
                txtbx_Voornaam.TextChanged -= txtbx_Voornaam_TextChanged;
                txtbx_Voornaam.Text = $"{Char.ToUpper(txtbx_Voornaam.Text.First())}{(txtbx_Voornaam.Text.Length > 1 ? txtbx_Voornaam.Text.Substring(1) : "")}";
                txtbx_Voornaam.Select(selectionStart, selectionLength);
                txtbx_Voornaam.TextChanged += txtbx_Voornaam_TextChanged;
            }
            oldText = txtbx_Voornaam.Text;
        }

        private void AutoCapitalCasingAchternaam() {
            string oldText = "";
            if ((txtbx_Achternaam.SelectionStart <= txtbx_Achternaam.Text.Length - oldText.Length
                || txtbx_Achternaam.SelectionStart == 0) &&
                char.IsLower(txtbx_Voornaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Achternaam.SelectionStart;
                var selectionLength = txtbx_Achternaam.SelectionLength;
                txtbx_Achternaam.TextChanged -= txtbx_Achternaam_TextChanged;
                txtbx_Achternaam.Text = $"{Char.ToUpper(txtbx_Achternaam.Text.First())}{(txtbx_Achternaam.Text.Length > 1 ? txtbx_Achternaam.Text.Substring(1) : "")}";
                txtbx_Achternaam.TextChanged += txtbx_Achternaam_TextChanged; ;
            }
            oldText = txtbx_Achternaam.Text;
        }

        private void Reset() {
            this.txtbx_Voornaam.Text = bestuurder.Voornaam;
            this.txtbx_Achternaam.Text = bestuurder.Achternaam;
            this.DatePckr_Geboortedatum.SelectedDate = bestuurder.GeboorteDatum;
            this.txtbx_RijksregisterNummer.Text = bestuurder.Rijksregisternummer;
            txtbx_RijksregisterNummeOud.Text = bestuurder.Rijksregisternummer;
            txtbx_VoornaamOud.Text = bestuurder.Achternaam;
            txtbx_AchternaamOud.Text = bestuurder.Voornaam;
            txtbx_GeboortedatumOud.Text = bestuurder.GeboorteDatum.ToShortDateString();
            if(bestuurder.rijbewijzen.Count > 0) {
                txtbx_RijbewijsOud.Text = string.Join(',', bestuurder.rijbewijzen.Select(x => x.ToString()));
                lbl_AantalRijbewijzen.Content = bestuurder.rijbewijzen.Count;
            } else { txtbx_RijbewijsOud.Text = "Geen rijbewijs!"; }
            if(bestuurder.Tankkaart != null) {
                txtbx_TankkaartOud.Text = string.Join(',', bestuurder.Tankkaart.Brandstoffen.Select(x => x.ToString()));
                lbl_TankkaartNummer.Content = bestuurder.Tankkaart.KaartNummer;
            } else { txtbx_TankkaartOud.Text = "Geen tankkaart!"; }
            
        }

        private void btn_Rijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rijbewijsSelectereb = new RijbewijsSelecteren();
            if (rijbewijsSelectereb.ShowDialog() == true) {
                this.rijbewijzen = rijbewijsSelectereb.Rijbewijzen;
                lbl_AantalRijbewijzen.Content = this.rijbewijzen.Count;
            }
        }

        private void btn_Tankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tankkaartSelecteren = new TankkaartSelecteren();
            if (tankkaartSelecteren.ShowDialog() == true) {
                this.tankkaart = tankkaartSelecteren.Tankkaart;
                lbl_TankkaartNummer.Content = this.tankkaart.KaartNummer;
            }
        }
    }
}
