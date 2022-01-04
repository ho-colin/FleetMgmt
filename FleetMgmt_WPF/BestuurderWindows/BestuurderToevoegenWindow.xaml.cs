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
        public Tankkaart tankkaart = null;

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
        private void AutoCapitalCasingAchternaam() {
            string oldText = "";
            if ((txtbx_achterNaam.SelectionStart <= txtbx_achterNaam.Text.Length - oldText.Length
                || txtbx_achterNaam.SelectionStart == 0) &&
                char.IsLower(txtbx_achterNaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_achterNaam.SelectionStart;
                var selectionLength = txtbx_achterNaam.SelectionLength;
                txtbx_achterNaam.TextChanged -= txtbx_achterNaam_TextChanged;
                txtbx_achterNaam.Text = $"{Char.ToUpper(txtbx_achterNaam.Text.First())}" +
                    $"{(txtbx_achterNaam.Text.Length > 1 ? txtbx_achterNaam.Text.Substring(1) : "")}";
                txtbx_achterNaam.Select(selectionStart, selectionLength);
                txtbx_achterNaam.TextChanged += txtbx_achterNaam_TextChanged;
            }
            oldText = txtbx_achterNaam.Text;
        }

        /// <summary>
        /// Start achternaam automatisch met hoofdletter
        /// </summary>
        private void AutoCapitalCasingVoornaam() {
            string oldText = "";
            if ((txtbx_Naam.SelectionStart <= txtbx_Naam.Text.Length - oldText.Length
                || txtbx_Naam.SelectionStart == 0) &&
                char.IsLower(txtbx_Naam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_Naam.SelectionStart;
                var selectionLength = txtbx_Naam.SelectionLength;
                txtbx_Naam.TextChanged -= txtbx_Naam_TextChanged;
                txtbx_Naam.Text = $"{Char.ToUpper(txtbx_Naam.Text.First())}" +
                    $"{(txtbx_Naam.Text.Length > 1 ? txtbx_Naam.Text.Substring(1) : "")}";
                txtbx_Naam.Select(selectionStart, selectionLength);
                txtbx_Naam.TextChanged += txtbx_Naam_TextChanged;
            }
            oldText = txtbx_Naam.Text;
        }

        private void Reset() {
            this.txtbx_achterNaam.Text = "";
            this.txtbx_Naam.Text = "";
            this.txtbx_Geldigheidsdatum.Text = "";
            this.txtbx_Rijksregisiternummer.Text = "";
        }

        private void btn_BestuurderToevoegen_Click(object sender, RoutedEventArgs e) {
            try {
                string naam1 = txtbx_achterNaam.Text;
                string naam2 = txtbx_Naam.Text;
                DateTime tijd = Convert.ToDateTime(txtbx_Geldigheidsdatum.Text);
                string rijksregisternummer = txtbx_Rijksregisiternummer.Text;
                if (naam1 == null) { MessageBox.Show("Gelieve een voornaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (naam2 == null) { MessageBox.Show("Gelieve een achternaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (tijd.GetHashCode() == 0) MessageBox.Show("Gelieve een geldige datum te kiezen!", "ERROR", MessageBoxButton.OK);
                if (rijksregisternummer == null) 
                    MessageBox.Show("Gelieve een rijksregisternummer in te geven!", "ERROR3", MessageBoxButton.OK);
                Bestuurder bestuurder = new Bestuurder(rijksregisternummer, naam1, naam2, tijd);
                if(tankkaart != null) { bestuurder.updateTankkaart(tankkaart); }
                Bestuurder besuurderTwee = _bestuurderManager.voegBestuurderToe(bestuurder);
                bestuurders.Add(besuurderTwee);
                lstVw_Bestuurders.ItemsSource = bestuurders;
                MessageBox.Show($"Bestuurder: {bestuurder.Achternaam} {bestuurder.Voornaam} werd zonet toegevoegd!", "Bestuurder toevoegen", MessageBoxButton.OK);
                Reset();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Tankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tankaartSelecteren = new TankkaartSelecteren();
            if(tankaartSelecteren.ShowDialog() == true) {
                this.tankkaart = tankaartSelecteren.Tankkaart;
                lbl_Tankkaart.Content = this.tankkaart.KaartNummer;
            }
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rijbewijsSelecteren = new RijbewijsSelecteren();
            if(rijbewijsSelecteren.ShowDialog() == true) {
                this.rijbewijzen = rijbewijsSelecteren.Rijbewijzen;
                lbl_Rijbewijs.Content = this.rijbewijzen.Count;
            }
        }

        private void txtbx_achterNaam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingAchternaam();
        }

        private void txtbx_Naam_TextChanged(object sender, TextChangedEventArgs e) {
            AutoCapitalCasingVoornaam();
        }
    }
}
