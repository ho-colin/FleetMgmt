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

namespace FleetMgmt_WPF.RijbewijsWindows {
    /// <summary>
    /// Interaction logic for ToevoegenRijbewijsWindow.xaml
    /// </summary>
    public partial class ToevoegenRijbewijsWindow : Window {


        private Bestuurder bestuurder { get; set; }
        RijbewijsManager rijbewijsManager = new RijbewijsManager(new RijbewijsRepository());

        public ToevoegenRijbewijsWindow(Bestuurder b) {
            InitializeComponent();
            PopulateCombobox();
            bestuurder = b;
            Reset();
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void btn_VoegRijbewijsToe_Click(object sender, RoutedEventArgs e) {
            try {
                if (cmbbx_Rijbewijs.SelectedItem == null && dtpckr_BehaaldOp.SelectedDate == null) MessageBox.Show("Er werd geen categorie" +
                     "en datum geselecteerd!", "Rijbewijs toevoegen", MessageBoxButton.OK);
                RijbewijsEnum rijbewijsKeuze = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), cmbbx_Rijbewijs.SelectedItem.ToString());
                Rijbewijs rijbewijs = new Rijbewijs(rijbewijsKeuze.ToString(), dtpckr_BehaaldOp.SelectedDate.Value);
                rijbewijs.zetBehaaldOp(dtpckr_BehaaldOp.SelectedDate.Value);
                rijbewijsManager.voegRijbewijsToe(rijbewijs, bestuurder);
                MessageBox.Show($"{rijbewijsKeuze} werd zonet toegevoegd aan {bestuurder.Voornaam} {bestuurder.Achternaam}");
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void PopulateCombobox() {
            cmbbx_Rijbewijs.ItemsSource = Enum.GetValues(typeof(RijbewijsEnum));
        }

        private void Reset() {
            lbl_GeselecteerdeBestuurder.Content =  bestuurder.Voornaam + " " + bestuurder.Achternaam;
            cmbbx_Rijbewijs.SelectedItem = null;
            dtpckr_BehaaldOp.SelectedDate = null;
        }
    }
}
