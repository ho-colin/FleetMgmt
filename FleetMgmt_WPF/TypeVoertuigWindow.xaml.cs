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
    /// Interaction logic for TypeVoertuigWindow.xaml
    /// </summary>
    public partial class TypeVoertuigWindow : Window {
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
            this.txtbx_TypeInput = null;
            this.combobx_VereistRijbewijs = null;
        }
    }
}
