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

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for VoertuigWindow.xaml
    /// </summary>
    public partial class VoertuigWindow : Window {
        public VoertuigWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        /// <summary>
        /// Navigatie naar bestuurdersWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            BestuurderWindow bw = new BestuurderWindow();
            bw.Show();
            this.Close();
        }

        /// <summary>
        /// Navigatie naat TypeVoertuigWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tvw = new TypeVoertuigWindow();
            tvw.Show();
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
    }
}
