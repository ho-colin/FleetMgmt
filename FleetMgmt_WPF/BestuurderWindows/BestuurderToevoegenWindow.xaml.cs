using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Validators;
using FleetMgmg_Data.Repositories;
using FleetMgmt_WPF.TankkaartWindows;
using FleetMgmt_WPF.RijbewijsWindows;
using FleetMgmt_Business.Enums;
using System.Collections.ObjectModel;

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for BestuurderToevoegenWindow.xaml
    /// </summary>
    public partial class BestuurderToevoegenWindow : Window {

        private BestuurderManager _bestuurderManager = new BestuurderManager(new BestuurderRepository());
        public List<Bestuurder> bestuurders = new List<Bestuurder>();
        public List<RijbewijsEnum> rijbewijzen = new List<RijbewijsEnum>();
        public ObservableCollection<Tankkaart> tankkaarten = new ObservableCollection<Tankkaart>();

        public BestuurderToevoegenWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            Reset();
        }


         /// <summary>
         /// Start voornaam automatisch met hoofdletter
         /// </summary>
        private void AutoCapitalCasingVoornaam() {
            string oldText = "";
            if ((txtbx_Voornaam.SelectionStart <= txtbx_Voornaam.Text.Length - oldText.Length
                || txtbx_Voornaam.SelectionStart == 0) &&
                char.IsLower(txtbx_Voornaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Voornaam.SelectionStart;
                var selectionLength = txtbx_Voornaam.SelectionLength;
                txtbx_Voornaam.TextChanged -= txtbx_Voornaam_TextChanged;
                txtbx_Voornaam.Text = $"{Char.ToUpper(txtbx_Voornaam.Text.First())}" +
                    $"{(txtbx_Voornaam.Text.Length > 1 ? txtbx_Voornaam.Text.Substring(1) : "")}";
                txtbx_Voornaam.Select(selectionStart, selectionLength);
                txtbx_Voornaam.TextChanged += txtbx_Voornaam_TextChanged;
            }
            oldText = txtbx_Voornaam.Text;
        }

        /// <summary>
        /// Start achternaam automatisch met hoofdletter
        /// </summary>
        private void AutoCapitalCasingAchternaam() {
            string oldText = "";
            if ((txtbx_Naam.SelectionStart <= txtbx_Naam.Text.Length - oldText.Length
                || txtbx_Naam.SelectionStart == 0) &&
                char.IsLower(txtbx_Voornaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Naam.SelectionStart;
                var selectionLength = txtbx_Naam.SelectionLength;
                txtbx_Naam.TextChanged -= txtbx_AchterNaam_TextChanged;
                txtbx_Naam.Text = $"{Char.ToUpper(txtbx_Naam.Text.First())}" +
                    $"{(txtbx_Naam.Text.Length > 1 ? txtbx_Naam.Text.Substring(1) : "")}";
                txtbx_Naam.TextChanged += txtbx_AchterNaam_TextChanged;
            }
            oldText = txtbx_Naam.Text;
        }

        private void Reset() {
            this.txtbx_Voornaam.Text = "";
            this.txtbx_Naam.Text = "";
            this.txtbx_Geldigheidsdatum.Text = "";
            this.txtbx_Rijksregisiternummer.Text = "";
        }

        private void btn_BestuurderToevoegen_Click(object sender, RoutedEventArgs e) {
            try {
                string naam1 = txtbx_Voornaam.Text;
                string naam2 = txtbx_Naam.Text;
                DateTime tijd = Convert.ToDateTime(txtbx_Geldigheidsdatum.Text);
                string rijksregisternummer = txtbx_Rijksregisiternummer.Text;
                if (naam1 == null) { MessageBox.Show("Gelieve een voornaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (naam2 == null) { MessageBox.Show("Gelieve een achternaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (tijd.GetHashCode() == 0) MessageBox.Show("Gelieve een geldige datum te kiezen!", "ERROR", MessageBoxButton.OK);
                if (rijksregisternummer == null) 
                    MessageBox.Show("Gelieve een rijksregisternummer in te geven!", "ERROR3", MessageBoxButton.OK);
                Bestuurder bestuurder = new Bestuurder(rijksregisternummer, naam1, naam2, tijd);
                Bestuurder besuurderTwee = _bestuurderManager.voegBestuurderToe(bestuurder);
                bestuurders.Add(besuurderTwee);
                lstVw_Bestuurders.ItemsSource = bestuurders;
                MessageBox.Show($"Bestuurder: {bestuurder.Achternaam} {bestuurder.Voornaam} werd zonet toegevoegd!", "Bestuurder toevoegen", MessageBoxButton.OK);
                Reset();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }


        private void txtbx_Voornaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingVoornaam();
        }


        private void txtbx_AchterNaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingAchternaam();
        }

        private void btn_Tankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tankaartSelecteren = new TankkaartSelecteren();
            if(tankaartSelecteren.ShowDialog() == true) {
                this.tankkaarten = tankaartSelecteren.tankkaarten;
                lbl_Tankkaart.Content = this.tankkaarten.Count;
            }
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rijbewijsSelecteren = new RijbewijsSelecteren();
            if(rijbewijsSelecteren.ShowDialog() == true) {
                this.rijbewijzen = rijbewijsSelecteren.Rijbewijzen;
                lbl_Rijbewijs.Content = this.rijbewijzen.Count;
            }
        }
    }
}
