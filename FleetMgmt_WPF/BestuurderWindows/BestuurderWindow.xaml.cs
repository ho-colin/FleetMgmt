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

namespace FleetMgmt_WPF {
    /// <summary>
    /// Interaction logic for BestuurderWindow.xaml
    /// </summary>

    public partial class BestuurderWindow : Window {
        public BestuurderWindow() {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }



        /// <summary>
        /// Deze methode zorgt ervoor dat wanneer de input van bv naam, voornaam louis is dat deze dan wordt omgezet naar Louis aan de hand van regex!
        /// de functie vervangt de eerste letter door een hoofdletter, deze functie wordt pas uitgevoerd wanneer de persoon een naam zou ingeven 
        /// die niet met hoofdletter begint
        //Deze methode is zeker handig wanneer men checks wil uitvoeren op textboxen
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string SetFirstLetterToUpperCase(string firstLetter) {
            return Regex.Replace(firstLetter, @"\b[a-z]\w+", delegate (Match match) {
                string v = match.ToString();
                return char.ToUpper(v[0]) + v.Substring(1);
            });
        }

        /// <summary>
        /// Deze methode checkt aan de hand van regex of er een geldige datum wordt weeergegeven
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxGeboorteDatum_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex reg = new Regex("^\\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$/");
            e.Handled = reg.IsMatch(e.Text);
        }

        private void btn_VoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            VoertuigWindow vw = new VoertuigWindow();
            vw.Show();
            this.Close();
        }

        private void btn_TypeVoertuigNavigatie_Click(object sender, RoutedEventArgs e) {
            TypeVoertuigWindow tvw = new TypeVoertuigWindow();
            tvw.Show();
            this.Close();
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            resetVelden();
        }

        private void resetVelden() {
            this.txtbx_RijksregisterInput.Text = "";
            this.txtbx_VoornaamInput.Text = "";
            this.txtbx_NaamInput.Text = "";
            this.txtbx_GeboorteDatumInput.Text = "";
        }
    }
}
