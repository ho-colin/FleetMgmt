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
using FleetMgmt_WPF.RijbewijsWindows;
using FleetMgmt_WPF.TankkaartWindows;
using FleetMgmt_Business.Enums;

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for BestuurderWindow.xaml
    /// </summary>

    public partial class BestuurderWindow : Window {


        private BestuurderManager bm = new BestuurderManager(new BestuurderRepository());

        Tankkaart Tankkaart { get; set; }

        public List<RijbewijsEnum> Rijbewijzen { get; set; }

        private ObservableCollection<Bestuurder> bestuurdersLijst = new ObservableCollection<Bestuurder>();

        public BestuurderWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btn_BestuurderToevegen_Click(object sender, RoutedEventArgs e) {
            BestuurderToevoegenWindow btw = new BestuurderToevoegenWindow();
            btw.Show();
        }

        private void btn_BestuurderZoeken_Click(object sender, RoutedEventArgs e) {
            try {
                string gevondenRijks = string.IsNullOrWhiteSpace(txtbx_Rijksregisternummer.Text) ? null : txtbx_Rijksregisternummer.Text;
                string gevondenAchterNaam = string.IsNullOrWhiteSpace(txtbx_Achternaam.Text) ? null : txtbx_Achternaam.Text;
                string gevondenVoornaam = string.IsNullOrWhiteSpace(txtbx_Voornaam.Text) ? null : txtbx_Voornaam.Text;
                DateTime? geboortedatum = 
                    Convert.ToDateTime(dtpckr_Geboortedatum.SelectedDate.HasValue ? 
                    dtpckr_Geboortedatum.SelectedDate.Value : null);
                bestuurdersLijst = new ObservableCollection<Bestuurder>(bm.toonBestuurders(gevondenRijks, gevondenAchterNaam, gevondenVoornaam, geboortedatum).ToList());
                lstVw_Bestuurders.ItemsSource = bestuurdersLijst;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void Reset() {
            this.txtbw_Id.Text = "";
            this.txtbx_Voornaam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.txtbx_Rijksregisternummer.Text = "";
            this.dtpckr_Geboortedatum.SelectedDate = null;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void btn_SelecteerTankkaart_Click(object sender, RoutedEventArgs e) {
            TankkaartSelecteren tks = new TankkaartSelecteren();
            if (tks.ShowDialog() == true) {
                this.Tankkaart = tks.Tankkaart;
                lbl_Tankkaart.Content = this.Tankkaart.KaartNummer;
            }
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rbs = new RijbewijsSelecteren();
            if (rbs.ShowDialog() == true) {
                this.Rijbewijzen = rbs.Rijbewijzen;
                lbl_Rijbewijs.Content = this.Rijbewijzen.Count;
            }
        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow vw = new VoertuigWindow();
            vw.Show();
            this.Close();
        }

        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }

        private void btn_TankkaartNavigatie_Click(object sender, RoutedEventArgs e) {
            TankkaartWindow tw = new TankkaartWindow();
            tw.Show();
            this.Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tyw = new TypeVoertuigWindow();
            tyw.Show();
            this.Close();
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
                MessageBoxResult result = MessageBox.Show($"Wenst u {bs.Voornaam} {bs.Achternaam} te verwijderen?", "Verwijder bestuurder",
                    MessageBoxButton.YesNo);
                switch (result) {
                    case MessageBoxResult.Yes:
                        bm.verwijderBestuurder(bs.Rijksregisternummer);
                        btn_BestuurderZoeken_Click(sender, e);
                        MessageBox.Show($"{bs.Voornaam} {bs.Achternaam} werd zonet verwijderd!");
                        break;
                    case MessageBoxResult.No:
                        MessageBox.Show($"{bs.Voornaam} {bs.Achternaam} wordt niet verwijderd!", "Verwijder bestuurder", 
                            MessageBoxButton.OK);
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
