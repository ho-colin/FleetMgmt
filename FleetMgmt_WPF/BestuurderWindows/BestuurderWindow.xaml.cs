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

        private void btn_BestuurderToevegen_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_BestuurderZoeken_Click(object sender, RoutedEventArgs e) {

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

        }

        private void btn_SelecteerRijbewijs_Click(object sender, RoutedEventArgs e) {
            RijbewijsSelecteren r = new RijbewijsSelecteren();
            r.Show();
            this.Close();
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

        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {

        }
    }
}
