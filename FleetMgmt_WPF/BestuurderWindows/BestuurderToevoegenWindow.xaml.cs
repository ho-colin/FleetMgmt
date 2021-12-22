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
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Validators;
using FleetMgmg_Data.Repositories;
using System.Text.RegularExpressions;
using FleetMgmt_WPF.TankkaartWindows;
using FleetMgmt_WPF.RijbewijsWindows;
using FleetMgmt_Business.Enums;
using System.Collections.ObjectModel;

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for BestuurderToevoegenWindow.xaml
    /// </summary>
    public partial class BestuurderToevoegenWindow : Window {

        BestuurderManager bm = new BestuurderManager(new BestuurderRepository());
        List<Bestuurder> bestuurders = new List<Bestuurder>();
        List<RijbewijsEnum> Rijbewijzen = new List<RijbewijsEnum>();
        ObservableCollection<Tankkaart> Tankkaarten = new ObservableCollection<Tankkaart>();

        public BestuurderToevoegenWindow() {
            InitializeComponent();
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            reset();
        }

        private void startVoorNaamMetHoofdletter() {
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

        private void startAchterNaamMetHoofdletter() {
            string oldText = "";
            if ((txtbx_AchterNaam.SelectionStart <= txtbx_AchterNaam.Text.Length - oldText.Length
                || txtbx_AchterNaam.SelectionStart == 0) &&
                char.IsLower(txtbx_Voornaam.Text.FirstOrDefault())) {
                var selectionStart = txtbx_AchterNaam.SelectionStart;
                var selectionLength = txtbx_AchterNaam.SelectionLength;
                txtbx_AchterNaam.TextChanged -= txtbx_AchterNaam_TextChanged;
                txtbx_AchterNaam.Text = $"{Char.ToUpper(txtbx_AchterNaam.Text.First())}{(txtbx_AchterNaam.Text.Length > 1 ? txtbx_AchterNaam.Text.Substring(1) : "")}";
                txtbx_AchterNaam.TextChanged += txtbx_AchterNaam_TextChanged;
            }
            oldText = txtbx_AchterNaam.Text;
        }

        private void reset() {
            this.txtbx_Voornaam.Text = "";
            this.txtbx_AchterNaam.Text = "";
            this.txtbx_Geldigheidsdatum.Text = "";
            this.txtbx_Rijksregisiternummer.Text = "";
        }

        private void btn_BestuurderToevoegen_Click(object sender, RoutedEventArgs e) {
            try {
                string Voornaam = txtbx_Voornaam.Text;
                string Achternaam = txtbx_AchterNaam.Text;
                DateTime tijd = Convert.ToDateTime(txtbx_Geldigheidsdatum.Text);
                string rijksregisternummer = txtbx_Rijksregisiternummer.Text;
                if (Voornaam == null) { MessageBox.Show("Gelieve een voornaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (Achternaam == null) { MessageBox.Show("Gelieve een achternaam in te vullen!", "ERROR", MessageBoxButton.OK); }
                if (tijd.GetHashCode() == 0) MessageBox.Show("Gelieve een geldige datum te kiezen!", "ERROR", MessageBoxButton.OK);
                if (rijksregisternummer == null) 
                    MessageBox.Show("Gelieve een rijksregisternummer in te geven!", "ERROR3", MessageBoxButton.OK);
                //Rijbewijs kan nog niet toegevoegd worden
                //Lijst van categoriën met de datum die er bij hoort.
                RijksregisterValidator.isGeldig(rijksregisternummer, tijd);
                Bestuurder bestuurder = new Bestuurder(rijksregisternummer, Voornaam, Achternaam, tijd);
                Bestuurder b2 = bm.voegBestuurderToe(bestuurder);
                bestuurders.Add(b2);
                lstVw_Bestuurders.ItemsSource = bestuurders;

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }


        private void txtbx_Voornaam_TextChanged(object sender, TextChangedEventArgs e) {
            startVoorNaamMetHoofdletter();
        }


        private void txtbx_AchterNaam_TextChanged(object sender, TextChangedEventArgs e) {
            startAchterNaamMetHoofdletter();
        }

        public static bool isIdValid(string s) {
            int i;
            return int.TryParse(s, out i) && i >= 0 && i <= int.MaxValue;
        }

        private void btn_Tankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tws = new TankkaartSelecteren();
            if(tws.ShowDialog() == true) {
                this.Tankkaarten = tws.tankkaarten;
                lbl_Tankkaart.Content = this.Tankkaarten.Count;
            }
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rs = new RijbewijsSelecteren();
            if(rs.ShowDialog() == true) {
                this.Rijbewijzen = rs.Rijbewijzen;
                lbl_Rijbewijs.Content = this.Rijbewijzen.Count;
            }
        }
    }
}
