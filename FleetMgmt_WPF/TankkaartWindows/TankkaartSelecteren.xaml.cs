using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.BestuurderWindows;
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

namespace FleetMgmt_WPF.TankkaartWindows {
    /// <summary>
    /// Interaction logic for TankkaartSelecteren.xaml
    /// </summary>

    public partial class TankkaartSelecteren : Window {

        TankkaartManager tm = new TankkaartManager(new TankkaartRepository());
        public ObservableCollection<Tankkaart> tankkaarten { get; set; } = new ObservableCollection<Tankkaart>();
        Bestuurder Bestuurder { get; set; }

        public Tankkaart Tankkaart { get; set; }

        public TankkaartSelecteren() {
            InitializeComponent();

            //Brandstof box populaten
            List<string> brandstoffen = new List<string>(Enum.GetNames(typeof(BrandstofEnum)).ToList());
            brandstoffen.Insert(0, "< Geen Brandstof >");
            cmbbx_Brandstof.ItemsSource = brandstoffen;
            

            //Geblokkeerd box populaten
            string[] gKeuzes = { "< Geen Keuze >", "Ja", "Nee" };
            cmbbx_Geblokkeerd.ItemsSource = gKeuzes;
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            txtbx_Id.Text = "";
            dtpckr_Geldigheidsdatum.SelectedDate = null;
            cmbbx_Geblokkeerd.SelectedIndex = 0;
            cmbbx_Brandstof.SelectedIndex = 0;
            this.Bestuurder = null;
            lbl_BestuurderNaam.Content = "";
            lstvw_Tankkaarten.ItemsSource = null;
            tankkaarten.Clear();
        }

        private void btn_Zoeken_Click(object sender, RoutedEventArgs e) {
            int? gevondenId = string.IsNullOrWhiteSpace(txtbx_Id.Text) ? null : int.Parse(txtbx_Id.Text);
            DateTime? gevondenDatum = dtpckr_Geldigheidsdatum.SelectedDate == null ? null : dtpckr_Geldigheidsdatum.SelectedDate.Value;
            string gevondenBestuurder = this.Bestuurder == null ? null : this.Bestuurder.Rijksregisternummer;
            bool? gevondenGeblokkeerd = cmbbx_Geblokkeerd.SelectedIndex == 0 ? null : cmbbx_Geblokkeerd.SelectedIndex == 1 ? true : false;
            TankkaartBrandstof? gevondenBrandstof = cmbbx_Brandstof.SelectedIndex == 0 ? null : (TankkaartBrandstof)Enum.Parse(typeof(BrandstofEnum),cmbbx_Brandstof.SelectedItem.ToString());
            this.tankkaarten = new ObservableCollection<Tankkaart>(tm.geefTankkaarten(gevondenId, gevondenDatum, gevondenBestuurder, gevondenGeblokkeerd, gevondenBrandstof));
            lstvw_Tankkaarten.ItemsSource = tankkaarten;
        }

        private void btn_Selecteren_Click(object sender, RoutedEventArgs e) {
            this.Tankkaart = (Tankkaart)lstvw_Tankkaarten.SelectedItem;
            DialogResult = true;
            this.Close();
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow w = new SelecteerBestuurderWindow();
            if(w.ShowDialog() == true) {
                this.Bestuurder = w.Bestuurder;
                lbl_BestuurderNaam.Content = this.Bestuurder.Naam;
            }
        }

        private void lstvw_Tankkaarten_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(lstvw_Tankkaarten.SelectedItem == null) {
                btn_Selecteren.IsEnabled = false;
            }else { btn_Selecteren.IsEnabled = true; }
        }
    }
}
