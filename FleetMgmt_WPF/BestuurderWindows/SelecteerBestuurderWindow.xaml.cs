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

namespace FleetMgmt_WPF.BestuurderWindows {
    /// <summary>
    /// Interaction logic for SelecteerBestuurderWindow.xaml
    /// </summary>
    public partial class SelecteerBestuurderWindow : Window {
        public List<Bestuurder> Bestuurders { get; set; }


        public SelecteerBestuurderWindow() {
            InitializeComponent();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e) {
            reset();
        }

        private void reset() {
            this.txtbx_Id.Text= "";
            this.txtbx_Naam.Text = "";
            this.txtbx_Achternaam.Text = "";
            this.Date_Pckr_Geboortedatum.Text = "";
            this.txtbx_rijksregsterNummer.Text = "";
        }

        private void btb_KeerTerug_Click(object sender, RoutedEventArgs e) {
            Hide();
            new SelecteerBestuurderWindow().ShowDialog();
            ShowDialog();
        }

        private void btn_Zoeken_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_Selecteren_Click(object sender, RoutedEventArgs e) {
            //Misschien hantereen om in plaats van een button, een dubbelklik op de row van de listview om iets te selecteren
        }
    }
}
