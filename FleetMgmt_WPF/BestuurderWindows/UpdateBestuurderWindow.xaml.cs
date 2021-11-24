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
    /// Interaction logic for UpdateBestuurderWindow.xaml
    /// </summary>
    public partial class UpdateBestuurderWindow : Window {
        public UpdateBestuurderWindow() {
            InitializeComponent();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            this.txtbx_Voornaam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.DatePckr_Geboortedatum.SelectedDate = null;
            this.txtbx_Id.Text = "";
            this.txtbx_RijksregisterNummer.Text = "";
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_Return_Click(object sender, RoutedEventArgs e) {

        }

        private void txtbx_Id_PreviewTextInput(object sender, TextCompositionEventArgs e) {

        }

        private void txtbx_Voornaam_TextChanged(object sender, TextChangedEventArgs e) {
            startVoorNaamMetHoofdletter();
        }

        private void txtbx_Achternaam_TextChanged(object sender, TextChangedEventArgs e) {
            startAchterNaamMetHoofdletter();
        }

        private void txtbx_RijksregisterNummer_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"^\d+$")) {
                e.Handled = true;
            }
        }

        private void startVoorNaamMetHoofdletter() {
            string oldText = "";
            //Naam begint met hoofdletter!
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
            //Naam begint met hoofdletter!
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

        private void txtbx_Achternaam_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }

        private void txtbx_Voornaam_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }
    }
}
