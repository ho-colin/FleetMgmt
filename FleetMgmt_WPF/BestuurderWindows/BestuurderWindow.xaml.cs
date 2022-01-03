using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Managers;
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

        private BestuurderManager _bestuurdersManager = new BestuurderManager(new BestuurderRepository());
        public Tankkaart tankkaart { get; set; }
        public List<RijbewijsEnum> rijbewijzen { get; set; }
        public ObservableCollection<Bestuurder> bestuurders = new ObservableCollection<Bestuurder>();

        public BestuurderWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btn_BestuurderToevegen_Click(object sender, RoutedEventArgs e) {
            BestuurderToevoegenWindow bestuurderToevoegen = new BestuurderToevoegenWindow();
            bestuurderToevoegen.Show();
        }

        private void btn_BestuurderZoeken_Click(object sender, RoutedEventArgs e) {
            try {
                string gevondenRijks = string.IsNullOrWhiteSpace(txtbx_Rijksregisternummer.Text) ? null : txtbx_Rijksregisternummer.Text;
                string gevondenAchterNaam = string.IsNullOrWhiteSpace(txtbx_Achternaam.Text) ? null : txtbx_Achternaam.Text;
                string gevondenVoornaam = string.IsNullOrWhiteSpace(txtbx_Voornaam.Text) ? null : txtbx_Voornaam.Text;
                DateTime? geboortedatum = 
                    Convert.ToDateTime(dtpckr_Geboortedatum.SelectedDate.HasValue ? 
                    dtpckr_Geboortedatum.SelectedDate.Value : null);
                bestuurders = new ObservableCollection<Bestuurder>
                    (_bestuurdersManager.toonBestuurders(gevondenRijks, gevondenAchterNaam, gevondenVoornaam, geboortedatum).ToList());
                lstVw_Bestuurders.ItemsSource = bestuurders;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void Reset() {
            this.txtbx_Voornaam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.txtbx_Rijksregisternummer.Text = "";
            this.dtpckr_Geboortedatum.SelectedDate = null;
            this.lstVw_Bestuurders.ItemsSource = null;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void btn_SelecteerTankkaart_Click(object sender, RoutedEventArgs e) {

            TankkaartSelecteren tankkaartSelecteren = new TankkaartSelecteren();
            if (tankkaartSelecteren.ShowDialog() == true) {
                this.tankkaart = tankkaartSelecteren.Tankkaart;
                lbl_Tankkaart.Content = this.tankkaart.KaartNummer;
            }
        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren rijbewijsSelecteren = new RijbewijsSelecteren();
            if (rijbewijsSelecteren.ShowDialog() == true) {
                this.rijbewijzen = rijbewijsSelecteren.Rijbewijzen;
                lbl_Rijbewijs.Content = this.rijbewijzen.Count;
            }
        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow voertuigWindow = new VoertuigWindow();
            voertuigWindow.Show();
            this.Close();
        }

        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bestuurderWindow = new BestuurderWindow();
            bestuurderWindow.Show();
            this.Close();
        }

        private void btn_TankkaartNavigatie_Click(object sender, RoutedEventArgs e) {
            TankkaartWindow tankkaartWindow = new TankkaartWindow();
            tankkaartWindow.Show();
            this.Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow typeVoertugWindow = new TypeVoertuigWindow();
            typeVoertugWindow.Show();
            this.Close();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                Bestuurder bestuurder = (Bestuurder)lstVw_Bestuurders.SelectedItem;
                UpdateBestuurderWindow updateBestuurder = new UpdateBestuurderWindow(bestuurder);
                if (updateBestuurder.ShowDialog() == true) {
                    btn_BestuurderZoeken_Click(sender, e);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            try {
                Bestuurder bestuurder = (Bestuurder)lstVw_Bestuurders.SelectedItem;
                MessageBoxResult result = MessageBox.Show($"Wenst u {bestuurder.Voornaam} {bestuurder.Achternaam}" +
                    $" te verwijderen?", "Verwijder bestuurder",
                    MessageBoxButton.YesNo);
                switch (result) {
                    case MessageBoxResult.Yes:
                        _bestuurdersManager.verwijderBestuurder(bestuurder);
                        btn_BestuurderZoeken_Click(sender, e);
                        MessageBox.Show($"{bestuurder.Voornaam} {bestuurder.Achternaam} werd zonet verwijderd!");
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
