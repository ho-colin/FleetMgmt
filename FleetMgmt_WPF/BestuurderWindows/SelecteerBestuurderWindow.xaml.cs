using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.RijbewijsWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for SelecteerBestuurderWindow.xaml
    /// </summary>
    public partial class SelecteerBestuurderWindow : Window {
        public List<Bestuurder> Bestuurders { get; set; }

        public Bestuurder Bestuurder = null;

        List<RijbewijsEnum> Rijbewijzen = new List<RijbewijsEnum>();

        private BestuurderManager bm = new BestuurderManager(new BestuurderRepository());

        private ObservableCollection<Bestuurder> bestuurdersLijst = new ObservableCollection<Bestuurder>();


        public SelecteerBestuurderWindow() {
            InitializeComponent();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            reset();
        }

        private void reset() {
            this.txtbx_Id.Text= "";
            this.txtbx_Naam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.Date_Pckr_Geboortedatum.Text = "";
            this.txtbx_rijksregsterNummer.Text = "";
        }

        private void btn_Zoeken_Click(object sender, RoutedEventArgs e) {
            int rijks = int.Parse(txtbx_rijksregsterNummer.Text);
            string voornaam = txtbx_Naam.Text;
            string achternaam = txtbx_Achternaam.Text;
            DateTime geboortedatum = Convert.ToDateTime(Date_Pckr_Geboortedatum.Text);
            try {
                bestuurdersLijst = new ObservableCollection<Bestuurder>(bm.toonBestuurders(rijks.ToString(), voornaam, achternaam, geboortedatum).ToList());
                lstVw_Bestuurders.ItemsSource = bestuurdersLijst;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void txtbx_Id_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !isIdValid(((TextBox)sender).Text + e.Text);
        }

        private void txtbx_Naam_TextChanged(object sender, TextChangedEventArgs e) {
            startVoorNaamMetHoofdletter();
        }

        private void txtbx_Achternaam_TextChanged(object sender, TextChangedEventArgs e) {
            startAchterNaamMetHoofdletter();
        }

        private void txtbx_rijksregsterNummer_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !isIdValid(((TextBox)sender).Text + e.Text);
        }


        private void startVoorNaamMetHoofdletter() {
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

        private void startAchterNaamMetHoofdletter() {
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

        private void txtbx_Naam_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }

        private void txtbx_Achternaam_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }

        public static bool isIdValid(string s) {
            int i;
            return int.TryParse(s, out i) && i >= 0 && i <= 100000000;
        }

        private void btn_Selecteren_Click(object sender, RoutedEventArgs e) {
            this.Bestuurder = (Bestuurder)lstVw_Bestuurders.SelectedItem;
            DialogResult = true;
            this.Close();
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rbs = new RijbewijsSelecteren();
            if (rbs.ShowDialog() == true)
                this.Rijbewijzen = rbs.Rijbewijzen;
            lbl_Rijbewijs.Content = this.Rijbewijzen.Count;
        }

        private void btn_SelecteerTankkaart_Click(object sender, RoutedEventArgs e) {

        }
    }
}
