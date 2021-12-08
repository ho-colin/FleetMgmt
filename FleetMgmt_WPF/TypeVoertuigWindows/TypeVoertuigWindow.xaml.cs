using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_WPF.TypeVoertuigWindows;
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
    /// Interaction logic for TypeVoertuigWindow.xaml
    /// </summary>
    public partial class TypeVoertuigWindow : Window {

        ObservableCollection<TypeVoertuig> TypeVoertuigen = new ObservableCollection<TypeVoertuig>();

        TypeVoertuigManager tvm = new TypeVoertuigManager(new TypeVoertuigRepository());

        public TypeVoertuigWindow() {


            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        /// <summary>
        /// Navigeer naar voertuigwindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow vw = new VoertuigWindow();
            vw.Show();
            this.Close();
        }

        /// <summary>
        /// Navigeer naar BestuurdersWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BestuurderNavigatie_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }

        /// <summary>
        /// Zet de waarden van de velden op null = RESET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            this.txtbx_TypeInput.Text = "";
            this.combobx_VereistRijbewijs.SelectedIndex = 0;
        }

        private void btn_TypeVoertuigZoeken_Click(object sender, RoutedEventArgs e) {
            string gevondenType = string.IsNullOrWhiteSpace(txtbx_TypeInput.Text) ? null : txtbx_TypeInput.Text;
            RijbewijsEnum? gevondenRijbewijs = (combobx_VereistRijbewijs.SelectedIndex != null) ? (RijbewijsEnum)combobx_VereistRijbewijs.SelectedItem : null;

            try {
                this.TypeVoertuigen = new ObservableCollection<TypeVoertuig>(tvm.verkrijgTypeVoertuigen(gevondenType, gevondenRijbewijs));
                lstvw_TypeVoertuig.ItemsSource = this.TypeVoertuigen;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message,ex.GetType().Name);
            }
        }

        private void btn_TypeVoertuigToevoegen_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigToevoegen w = new TypeVoertuigToevoegen();
            w.Show();
        }

        private void populateRijbewijzen() {
            List<string> rijbewijzen = new List<string>(Enum.GetValues(typeof(RijbewijsEnum)).Cast<string>().ToList());
            rijbewijzen.Insert(0, "<Geen Rijbewijs>");
            combobx_VereistRijbewijs.ItemsSource = rijbewijzen;
            combobx_VereistRijbewijs.SelectedIndex = 0;
        }
    }
}
