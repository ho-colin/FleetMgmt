using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
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

        private void btb_KeerTerug_Click(object sender, RoutedEventArgs e) {
            Hide();
            new SelecteerBestuurderWindow().ShowDialog();
            ShowDialog();
        }

        private void btn_Zoeken_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_Selecteren_Click(object sender, RoutedEventArgs e) {
            //Misschien hantereen om in plaats van een button, een dubbelklik op de row van de listview om iets te selecteren
        }

        private void txtbx_Id_PreviewTextInput(object sender, TextCompositionEventArgs e) {

        }

        private void txtbx_Naam_TextChanged(object sender, TextChangedEventArgs e) {
            startVoorNaamMetHoofdletter();
        }

        private void txtbx_Achternaam_TextChanged(object sender, TextChangedEventArgs e) {
            startAchterNaamMetHoofdletter();
        }

        private void txtbx_rijksregsterNummer_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"^\d+$")) {
                e.Handled = true;
            }
        }


        private void startVoorNaamMetHoofdletter() {
            string oldText = "";
            //Naam begint met hoofdletter!
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
            //Naam begint met hoofdletter!
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
    }
}
