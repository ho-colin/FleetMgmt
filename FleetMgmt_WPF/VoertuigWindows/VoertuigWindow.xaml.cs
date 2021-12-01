using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
using FleetMgmt_WPF.TankkaartWindows;
using FleetMgmt_WPF.VoertuigWindows;
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

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for VoertuigWindow.xaml
    /// </summary>
    public partial class VoertuigWindow : Window {
        VoertuigManager vm = new VoertuigManager(new VoertuigRepository());
        private Bestuurder b = null;
        private ObservableCollection<Voertuig> gevondenVoertuigen = new ObservableCollection<Voertuig>();
        public VoertuigWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

            combobx_Brandstof.ItemsSource = Enum.GetValues(typeof(BrandstofEnum));
        }

        /// <summary>
        /// Navigatie naar bestuurdersWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }
        /// <summary>
        /// Navigatie naar VoertuigWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("U begeeft zich momenteel in dit venster!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Navigatie naar TypeVoertuigWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tvw = new TypeVoertuigWindow();
            tvw.Show();
            this.Close();

        }
        /// <summary>
        /// Navigatie naar TankkaartWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_TankkaartNavigatie_Click(object sender, RoutedEventArgs e) {
            TankkaartWindow tkw = new TankkaartWindow();
            tkw.Show();
            this.Close();
        }

        /// <summary>
        /// Alle waarden van de input worden op null gezet = RESET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            this.txtbw_ChassisNummer = null;
            this.txtbx_Nummerplaat = null;
            this.txtbx_Merk = null;
            this.txtbx_Model = null;
            this.combobx_TypeVoertuig = null;
            this.combobx_Brandstof = null;
            this.txtbx_Kleur = null;
            this.txtbx_AantalDeuren = null;
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow stw = new SelecteerBestuurderWindow();
            stw.Show();
            this.Close();
        }

        private void btn_VoertuigToevoegen_Click(object sender, RoutedEventArgs e) {
            VoertuigToevoegenWindow w = new VoertuigToevoegenWindow();
            w.Show();
        }

        private void btn_VoertuigZoeken_Click(object sender, RoutedEventArgs e) {
            string gevondenChassis = string.IsNullOrWhiteSpace(txtbw_ChassisNummer.Text) ? null : txtbw_ChassisNummer.Text;
            string gevondenMerk = string.IsNullOrWhiteSpace(txtbx_Merk.Text) ? null : txtbx_Merk.Text;
            string gevondenModel = string.IsNullOrWhiteSpace(txtbx_Model.Text) ? null : txtbx_Model.Text;
            string gevondenTypeVoertuig = null;//nog vragen
            BrandstofEnum? gevondenBrandstof = combobx_Brandstof.SelectedItem == null ? null : (BrandstofEnum)combobx_Brandstof.SelectedItem;
            string gevondenKleur = string.IsNullOrWhiteSpace(txtbx_Kleur.Text) ? null : txtbx_Kleur.Text;
            int? gevondenAantalDeuren = string.IsNullOrWhiteSpace(txtbx_AantalDeuren.Text) ? null : int.Parse(txtbx_AantalDeuren.Text);
            string gevondenBestuurder = (this.b != null) ? this.b.Rijksregisternummer : null;
            try {
                gevondenVoertuigen = new ObservableCollection<Voertuig>(vm.zoekVoertuigen(
                    gevondenChassis, gevondenMerk, gevondenModel, gevondenTypeVoertuig, gevondenBrandstof, gevondenKleur, gevondenAantalDeuren,gevondenBestuurder));
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                Voertuig v = (Voertuig)dtgd_Voertuigen.SelectedItem;
                UpdateVoertuigWindow w = new UpdateVoertuigWindow(v);
                if(w.ShowDialog() == true) {
                    btn_VoertuigZoeken_Click(sender, e);
                }
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            try {
                Voertuig v = (Voertuig)dtgd_Voertuigen.SelectedItem;
                vm.verwijderVoertuig(v);
                btn_VoertuigToevoegen_Click(sender, e);
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
