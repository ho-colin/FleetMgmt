using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
using FleetMgmt_WPF.TankkaartWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FleetMgmt_WPF.RijbewijsWindows {
    /// <summary>
    /// Interaction logic for RijbewijsWindow.xaml
    /// </summary>
    public partial class RijbewijsWindow : Window {

        private Bestuurder _bestuurder = null;
        RijbewijsManager rijbewijsManager = new RijbewijsManager(new RijbewijsRepository());


        public RijbewijsWindow() {
            this.ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
        }

        private void btn_RijbewijsToevoegen_Click(object sender, RoutedEventArgs e) {
            ToevoegenRijbewijsWindow tv = new ToevoegenRijbewijsWindow(_bestuurder);
            if(tv.ShowDialog() == true) {
                lstVw_Rijbewijzen.ItemsSource = null;
                lstVw_Rijbewijzen.ItemsSource = _bestuurder.rijbewijzen;
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void Reset() {
            lblBestuurderRijbewijs.Content = "";
            lstVw_Rijbewijzen.ItemsSource = null;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            try {
                Rijbewijs geslecteerdRijbewijs = (Rijbewijs) lstVw_Rijbewijzen.SelectedItem;
                MessageBoxResult result = MessageBox.Show($"Wenst u het rijbewijs {geslecteerdRijbewijs.Categorie.ToString()} te " +
                    $"verwijderen van {_bestuurder.Voornaam}?","Rijbewijs verwijderen?",MessageBoxButton.YesNo);
                switch (result) {
                    case MessageBoxResult.Yes:
                        try {
                            rijbewijsManager.verwijderRijbewijs(geslecteerdRijbewijs.Categorie, _bestuurder);
                            MessageBox.Show($"Het {geslecteerdRijbewijs.ToString()} rijbewijs van {_bestuurder.Voornaam}" +
                                $"werd zonet verwijderd!");
                            _bestuurder.verwijderRijbewijs(geslecteerdRijbewijs);
                            lstVw_Rijbewijzen.ItemsSource = null;
                            lstVw_Rijbewijzen.ItemsSource = _bestuurder.rijbewijzen;
                            break;
                        } catch (Exception ex) {
                            MessageBox.Show(ex.Message);
                            break;
                        }
                    case MessageBoxResult.No:
                        break;
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }


        }

        private void btn_BestuurderSelecteren_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow selecteerBestuurderWindow = new SelecteerBestuurderWindow();
            if (selecteerBestuurderWindow.ShowDialog() == true) {
                this._bestuurder = selecteerBestuurderWindow.bestuurder;
                lblBestuurderRijbewijs.Content = this._bestuurder.Voornaam + " " + this._bestuurder.Achternaam;
                if (_bestuurder.rijbewijzen != null) {
                    lstVw_Rijbewijzen.ItemsSource = _bestuurder.rijbewijzen;
                }
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
            TypeVoertuigWindow typeVoertuigWindow = new TypeVoertuigWindow();
            typeVoertuigWindow.Show();
            this.Close();
        }

        private void btn_RijbewijsNavgiatie_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("U bevindt zich in dit venster", "Rijbewijs", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
