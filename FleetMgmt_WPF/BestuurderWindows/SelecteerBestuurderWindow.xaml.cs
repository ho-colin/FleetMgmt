using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.RijbewijsWindows;
using FleetMgmt_WPF.TankkaartWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for SelecteerBestuurderWindow.xaml
    /// </summary>

    public partial class SelecteerBestuurderWindow : Window {

        public List<Bestuurder> bestuurders { get; set; }
        public Bestuurder bestuurder = null;
        public Tankkaart tankkaart { get; set; }
        public List<RijbewijsEnum> rijbewijzen = new List<RijbewijsEnum>();
        private BestuurderManager _bestuurderManager = new BestuurderManager(new BestuurderRepository());
        public  ObservableCollection<Bestuurder> bestuurdersLijst = new ObservableCollection<Bestuurder>();



        public SelecteerBestuurderWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Reset() {
            this.txtbx_Naam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.Date_Pckr_Geboortedatum.Text = "";
            this.txtbx_rijksregsterNummer.Text = "";
            this.lstVw_Bestuurders.ItemsSource = null;
        }

        private void btn_Zoeken_Click(object sender, RoutedEventArgs e) {
            try {
                string gevondenRijks = string.IsNullOrWhiteSpace(txtbx_rijksregsterNummer.Text) ? null : txtbx_rijksregsterNummer.Text;
                string gevondenNaam = string.IsNullOrWhiteSpace(txtbx_Naam.Text) ? null : txtbx_Naam.Text;
                string gevondenAchternaam = string.IsNullOrWhiteSpace(txtbx_Achternaam.Text) ? null : txtbx_Achternaam.Text;
                DateTime? geboortedatum =
                    Convert.ToDateTime(Date_Pckr_Geboortedatum.SelectedDate.HasValue ?
                    Date_Pckr_Geboortedatum.SelectedDate.Value : null);
                bestuurdersLijst = new ObservableCollection<Bestuurder>(_bestuurderManager.toonBestuurders(gevondenRijks, gevondenNaam, gevondenAchternaam, geboortedatum).ToList());
                lstVw_Bestuurders.ItemsSource = bestuurdersLijst;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void txtbx_Naam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingVoornaam();
        }

        private void txtbx_Achternaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingAchternaam();
        }


        private void AutoCapitalCasingVoornaam() {
            string oldText = "";
            if ((txtbx_Naam.SelectionStart <= txtbx_Naam.Text.Length - oldText.Length
                || txtbx_Naam.SelectionStart == 0) &&
                char.IsLower(txtbx_Naam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Naam.SelectionStart;
                var selectionLength = txtbx_Naam.SelectionLength;
                txtbx_Naam.TextChanged -= txtbx_Naam_TextChanged;
                txtbx_Naam.Text = $"{Char.ToUpper(txtbx_Naam.Text.First())}{(txtbx_Naam.Text.Length > 1 ? txtbx_Naam.Text.Substring(1) : "")}";
                txtbx_Naam.Select(selectionStart, selectionLength);
                txtbx_Naam.TextChanged += txtbx_Naam_TextChanged;
            }
            oldText = txtbx_Naam.Text;
        }

        private void AutoCapitalCasingAchternaam() {
            string oldText = "";
            if ((txtbx_Achternaam.SelectionStart <= txtbx_Achternaam.Text.Length - oldText.Length
                || txtbx_Achternaam.SelectionStart == 0) &&
                char.IsLower(txtbx_Achternaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Achternaam.SelectionStart;
                var selectionLength = txtbx_Achternaam.SelectionLength;
                txtbx_Achternaam.TextChanged -= txtbx_Achternaam_TextChanged;
                txtbx_Achternaam.Text = $"{Char.ToUpper(txtbx_Achternaam.Text.First())}{(txtbx_Achternaam.Text.Length > 1 ? txtbx_Achternaam.Text.Substring(1) : "")}";
                txtbx_Achternaam.TextChanged += txtbx_Achternaam_TextChanged;
            }
            oldText = txtbx_Achternaam.Text;
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {

            RijbewijsSelecteren rijbewijsSelecteren = new RijbewijsSelecteren();
            if (rijbewijsSelecteren.ShowDialog() == true)
                this.rijbewijzen = rijbewijsSelecteren.Rijbewijzen;
            lbl_Rijbewijs.Content = this.rijbewijzen.Count;
        }

        private void btn_SelecteerTankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tankkaartSelecteren = new TankkaartSelecteren();
            if (tankkaartSelecteren.ShowDialog() == true)
                this.tankkaart = tankkaartSelecteren.Tankkaart;
            lbl_Tankkaart.Content = this.tankkaart.KaartNummer;
        }

        private void lstVw_Bestuurders_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(lstVw_Bestuurders.SelectedItem == null) {
                btn_Selecteren.IsEnabled = false;
            }else { btn_Selecteren.IsEnabled = true; }
        }

        private void btn_Selecteren_Click_1(object sender, RoutedEventArgs e) {
            this.bestuurder = (Bestuurder)lstVw_Bestuurders.SelectedItem;
            DialogResult = true;
            this.Close();
        }

        private void btn_Reset_Click_1(object sender, RoutedEventArgs e) {
            Reset();
        }
    }
}
