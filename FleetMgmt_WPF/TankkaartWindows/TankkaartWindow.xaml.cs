using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
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
using FleetMgmt_WPF.BestuurderWindows;

namespace FleetMgmt_WPF.TankkaartWindows {
    /// <summary>
    /// Interaction logic for TankkaartWindow.xaml
    /// </summary>
    public partial class TankkaartWindow : Window {

        TankkaartManager tm = new TankkaartManager(new TankkaartRepository());

        private FleetMgmt_Business.Objects.Bestuurder b = null;

        private ObservableCollection<FleetMgmt_Business.Objects.Tankkaart> gevondenTankkaarten = new ObservableCollection<FleetMgmt_Business.Objects.Tankkaart>();

        public TankkaartWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

            List<string> combobxgeblokkeerd = new List<String>() { "<Leeg>", "Ja", "Nee" };
            combobx_Geblokkeerd.ItemsSource = combobxgeblokkeerd;

            combobx_Brandstof.ItemsSource = Enum.GetValues(typeof(TankkaartBrandstof));
        }

        private void btn_SelecteerBestuurder_Click(object sender, RoutedEventArgs e) {
            SelecteerBestuurderWindow stw = new SelecteerBestuurderWindow();
            stw.Show();
            this.Close();
        }

        private void btn_TankkaartToevoegen_Click(object sender, RoutedEventArgs e) {
            TankkaartToevoegen w = new TankkaartToevoegen();
            w.Show();
        }

        private void btn_TankkaartZoeken_Click(object sender, RoutedEventArgs e) {
            int? gevondenId = string.IsNullOrWhiteSpace(txtbw_Id.Text) ? null : int.Parse(txtbw_Id.Text);
            DateTime? gevondenDatum = txtbx_Geldigheidsdatum.SelectedDate.HasValue ? txtbx_Geldigheidsdatum.SelectedDate.Value : null;
            FleetMgmt_Business.Enums.TankkaartBrandstof? gevondenBrandstof = combobx_Brandstof.SelectedItem == null ? null : (FleetMgmt_Business.Enums.TankkaartBrandstof) combobx_Brandstof.SelectedItem;
            string gevondenBestuurder = (this.b != null) ? this.b.Rijksregisternummer : null;

            try {
                gevondenTankkaarten = new ObservableCollection<FleetMgmt_Business.Objects.Tankkaart>(tm.geefTankkaarten(gevondenId, gevondenDatum, gevondenBestuurder, checkDisabled(), gevondenBrandstof).ToList());
                lstVw_Tankkaarten.ItemsSource = gevondenTankkaarten;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void resetVelden() {
            txtbw_Id.Text = "";
            txtbx_Geldigheidsdatum.SelectedDate = null;
            combobx_Geblokkeerd.SelectedIndex = 0;
            b = null;
            lbl_BestuurderNaam.Content = "";
            gevondenTankkaarten.Clear();
            lstVw_Tankkaarten.ItemsSource = gevondenTankkaarten;
            combobx_Brandstof.SelectedItem = null;
            
        }

        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            //IMPLEMENTEREN NAAR BESTUURDER
            Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            //IMPLEMENTEREN NAAR TYPEVOERTUIG
            Close();
        }

        private void btn_TankkaartNavigatie_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("U begeeft zich momenteel in dit venster!","Error",MessageBoxButton.OK,MessageBoxImage.Error);

        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            //IMPLEMENTEREN NAAR VOERTUIG
            Close();
        }

        private bool? checkDisabled() {
            if(combobx_Geblokkeerd.SelectedIndex == 1) { return true; }
            if (combobx_Geblokkeerd.SelectedIndex == 2) { return false; } 
            else return null;
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e) {
            try {
                FleetMgmt_Business.Objects.Tankkaart tk = (FleetMgmt_Business.Objects.Tankkaart)lstVw_Tankkaarten.SelectedItem;
                TankkaartUpdaten w = new TankkaartUpdaten(tk);
                if(w.ShowDialog() == true) {
                    btn_TankkaartZoeken_Click(sender, e);
                }
            } catch (Exception) {

                throw;
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e) {
            try {
                FleetMgmt_Business.Objects.Tankkaart tk = (FleetMgmt_Business.Objects.Tankkaart)lstVw_Tankkaarten.SelectedItem;
                tm.verwijderTankkaart(tk.KaartNummer);
                btn_TankkaartZoeken_Click(sender, e);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void txtbw_Id_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^\d+$")) {
                e.Handled = true;
            }
        }
    }
}
