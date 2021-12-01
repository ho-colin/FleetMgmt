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
using System.Text.RegularExpressions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Repos;
using FleetMgmg_Data.Repositories;
using FleetMgmt_WPF.BestuurderWindows;
using System.Collections.ObjectModel;

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for BestuurderWindow.xaml
    /// </summary>

    public partial class BestuurderWindow : Window {


        private BestuurderManager bm = new BestuurderManager(new BestuurderRepository());

        private ObservableCollection<Bestuurder> bestuurdersLijst = new ObservableCollection<Bestuurder>();

        public BestuurderWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }
        
        private void textBoxGeboorteDatum_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex reg = new Regex("^\\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$/");
            e.Handled = reg.IsMatch(e.Text);
        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow vw = new VoertuigWindow();
            vw.Show();
            this.Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tvw = new TypeVoertuigWindow();
            tvw.Show();
            this.Close();
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void resetVelden() {
            this.txtbx_RijksregisterInput.Text = "";
            this.txtbx_VoornaamInput.Text = "";
            this.txtbx_NaamInput.Text = "";
            this.dtpickr_Geboortedatum.SelectedDate = null;
        }

        private void txtbx_RijksregisterInput_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !isIdValid(((TextBox)sender).Text + e.Text);
        }

        private void txtbx_VoornaamInput_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }

        private void txtbx_NaamInput_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!Regex.IsMatch(e.Text, @"/[a-z]/gi")) {
                e.Handled = true;
            }
        }

        public static bool isIdValid(string s) {
            int i;
            return int.TryParse(s, out i) && i >= 0 && i <= 100000000;
        }

        private void startVoorNaamMetHoofdletter() {
            string oldText = "";
            if ((txtbx_VoornaamInput.SelectionStart <= txtbx_VoornaamInput.Text.Length - oldText.Length
                || txtbx_VoornaamInput.SelectionStart == 0) &&
                char.IsLower(txtbx_VoornaamInput.Text.FirstOrDefault())) {
                var selectionStart = txtbx_VoornaamInput.SelectionStart;
                var selectionLength = txtbx_VoornaamInput.SelectionLength;
                txtbx_VoornaamInput.TextChanged -= txtbx_VoornaamInput_TextChanged;
                txtbx_VoornaamInput.Text = $"{Char.ToUpper(txtbx_VoornaamInput.Text.First())}{(txtbx_VoornaamInput.Text.Length > 1 ? txtbx_VoornaamInput.Text.Substring(1) : "")}";
                txtbx_VoornaamInput.Select(selectionStart, selectionLength);
                txtbx_VoornaamInput.TextChanged += txtbx_VoornaamInput_TextChanged;
            }
            oldText = txtbx_VoornaamInput.Text;
        }


        private void startAchterNaamMetHoofdletter() {
            string oldText = "";
            if ((txtbx_NaamInput.SelectionStart <= txtbx_NaamInput.Text.Length - oldText.Length
                || txtbx_NaamInput.SelectionStart == 0) &&
                char.IsLower(txtbx_NaamInput.Text.FirstOrDefault())) {
                var selectionStart = txtbx_NaamInput.SelectionStart;
                var selectionLength = txtbx_NaamInput.SelectionLength;
                txtbx_NaamInput.TextChanged -= txtbx_NaamInput_TextChanged;
                txtbx_NaamInput.Text = $"{Char.ToUpper(txtbx_NaamInput.Text.First())}{(txtbx_NaamInput.Text.Length > 1 ? txtbx_NaamInput.Text.Substring(1) : "")}";
                txtbx_NaamInput.TextChanged += txtbx_NaamInput_TextChanged;
            }
            oldText = txtbx_NaamInput.Text;
        }

        private void txtbx_VoornaamInput_TextChanged(object sender, TextChangedEventArgs e) {
            startVoorNaamMetHoofdletter();
        }

        private void txtbx_NaamInput_TextChanged(object sender, TextChangedEventArgs e) {
            startAchterNaamMetHoofdletter();
        }

        private void btn_BestuurderZoeken_Click(object sender, RoutedEventArgs e) {
            string rijks = txtbx_RijksregisterInput.Text;
            string voornaam = txtbx_VoornaamInput.Text;
            string achternaam = txtbx_NaamInput.Text;
            DateTime geboortedatum = Convert.ToDateTime(dtpickr_Geboortedatum.SelectedDate.Value);
            try {
                bestuurdersLijst = new ObservableCollection<Bestuurder>(bm.toonBestuurders(rijks, achternaam, voornaam, geboortedatum).ToList());
                lstVw_Bestuurders.ItemsSource = bestuurdersLijst;
            }catch(Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                Bestuurder bst = (Bestuurder)lstVw_Bestuurders.SelectedItem;
                UpdateBestuurderWindow bws = new UpdateBestuurderWindow(bst);
                if (bws.ShowDialog() == true) {
                    btn_BestuurderZoeken_Click(sender, e);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            try {
                Bestuurder bs = (Bestuurder)lstVw_Bestuurders.SelectedItem;
                bm.verwijderBestuurder(bs.Id);
                btn_BestuurderZoeken_Click(sender, e);
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
