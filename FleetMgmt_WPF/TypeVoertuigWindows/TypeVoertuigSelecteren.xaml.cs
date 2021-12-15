using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
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

namespace FleetMgmt_WPF.TypeVoertuigWindows {
    /// <summary>
    /// Interaction logic for TypeVoertuigSelecteren.xaml
    /// </summary>
    public partial class TypeVoertuigSelecteren : Window {

        TypeVoertuigManager tvm = new TypeVoertuigManager(new TypeVoertuigRepository());
        ObservableCollection<TypeVoertuig> gevondenTypes = new ObservableCollection<TypeVoertuig>();

        public TypeVoertuig TypeVoertuig = null;

        public TypeVoertuigSelecteren() {
            InitializeComponent();
            populateRijbewijzen();
        }

        private void btn_ZoekTypeVoertuig_Click(object sender, RoutedEventArgs e) {
            string gevondenTypeVoertuig = string.IsNullOrWhiteSpace(txtbx_TypeVoertuig.Text) ? null : txtbx_TypeVoertuig.Text;
            RijbewijsEnum? gevondenRijbewijs = cmbobx_Rijbewijs.SelectedIndex == 0 ? null : (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), cmbobx_Rijbewijs.SelectedItem.ToString());

            try {
                gevondenTypes = new ObservableCollection<TypeVoertuig>(tvm.verkrijgTypeVoertuigen(gevondenTypeVoertuig, gevondenRijbewijs));
                lstvw_TypeVoertuig.ItemsSource = gevondenTypes;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btn_SelecteerTypeVoertuig_Click(object sender, RoutedEventArgs e) {
            this.TypeVoertuig = (TypeVoertuig)lstvw_TypeVoertuig.SelectedItem;
            DialogResult = true;
            this.Close();
        }

        private void lstvw_TypeVoertuig_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(lstvw_TypeVoertuig.SelectedItem == null) {
                btn_SelecteerTypeVoertuig.IsEnabled = false;
            }else { btn_SelecteerTypeVoertuig.IsEnabled = true; }
        }

        private void populateRijbewijzen() {
            List<string> rijbewijzen = new List<string>(Enum.GetNames(typeof(RijbewijsEnum)));
            rijbewijzen.Insert(0, "<Geen Rijbewijs>");
            cmbobx_Rijbewijs.ItemsSource = rijbewijzen;
            cmbobx_Rijbewijs.SelectedIndex = 0;
        }
    }
}
