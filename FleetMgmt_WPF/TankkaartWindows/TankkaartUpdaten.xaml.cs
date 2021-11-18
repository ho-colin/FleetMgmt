using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
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

namespace FleetMgmt_WPF.TankkaartWindows {
    /// <summary>
    /// Interaction logic for TankkaartUpdaten.xaml
    /// </summary>
    public partial class TankkaartUpdaten : Window {

        TankkaartManager tm = new TankkaartManager(new TankkaartRepository());

        FleetMgmt_Business.Objects.Tankkaart Tankkaart { get; set; }

        FleetMgmt_Business.Objects.Bestuurder Bestuurder = null;

        List<TankkaartBrandstof> Brandstoffen { get; set; }

        public TankkaartUpdaten(FleetMgmt_Business.Objects.Tankkaart tk) {
            this.Tankkaart = tk;

            if (this.Bestuurder != null) { this.Bestuurder = Tankkaart.InBezitVan; }
            this.Brandstoffen = Tankkaart.Brandstoffen;

            InitializeComponent();
            resetVelden();
        }

        private void btn_Brandstof_Click(object sender, RoutedEventArgs e) {
            BrandstofSelecteren w = new BrandstofSelecteren();
            if (w.ShowDialog() == true) {
                this.Brandstoffen = w.brandstoffen;
                lbl_NieuwBrandstoffen.Content = this.Brandstoffen.Count + " Brandstof(fen)";
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                Tankkaart newTankkaart = new Tankkaart(Tankkaart.KaartNummer, txtbx_Geldigheidsdatum.SelectedDate.Value, txtbx_NieuwPincode.Text, this.Bestuurder, Brandstoffen, chekbx_Geblokkeerd.IsChecked.Value);
                tm.bewerkTankkaart(newTankkaart);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void resetVelden() {
            try {
                this.Brandstoffen = Tankkaart.Brandstoffen;

                //Nieuwe waarden kollom//
                txtbx_NieuwId.Text = Tankkaart.KaartNummer.ToString();

                txtbx_Geldigheidsdatum.SelectedDate = Tankkaart.GeldigheidsDatum;

                chekbx_Geblokkeerd.IsChecked = Tankkaart.Geblokkeerd;

                lbl_NieuwBrandstoffen.Content = this.Brandstoffen.Count + " Brandstof(fen)";
               
                if(this.Bestuurder == null) {
                    lbl_Bestuurder.Content = "Geen Bestuurder";
                } else {
                    lbl_Bestuurder.Content = this.Bestuurder.Naam;
                }

                //Huidige waarden kollom//
                txtbx_HuidigId.Text = Tankkaart.KaartNummer.ToString();

                txtbx_HuidigGeldigheidsdatum.Text = Tankkaart.GeldigheidsDatum.ToString("dd/MM/yyyy");

                txtbx_HuidigGeblokkeerd.Text = Tankkaart.Geblokkeerd ? "Ja" : "Nee";

                textbx_HuidigBrandstoffen.Text = Tankkaart.Brandstoffen.Count.ToString();

                if (this.Bestuurder == null) {
                    textbx_HuidigBestuurder.Text = "Geen Bestuurder";
                } else {
                    textbx_HuidigBestuurder.Text = this.Bestuurder.Naam;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }
    }
}
