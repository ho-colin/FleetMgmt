using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
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
    /// Interaction logic for TypeVoertuigToevoegen.xaml
    /// </summary>
    public partial class TypeVoertuigToevoegen : Window {

        ObservableCollection<TypeVoertuig> typeVoertuigen = new ObservableCollection<TypeVoertuig>();

        TypeVoertuigManager tvm = new TypeVoertuigManager(new TypeVoertuigRepository());

        public TypeVoertuigToevoegen() {
            InitializeComponent();
        }

        private void btn_ResetVelden_Click(object sender, RoutedEventArgs e) {
            txtbx_TypeVoertuig.Text = "";
            combobx_Rijbewijs.SelectedIndex = 0;
        }

        private void btn_VoegTypeToe_Click(object sender, RoutedEventArgs e) {

            string gevondenType = txtbx_TypeVoertuig.Text;
            RijbewijsEnum gevondenRijbewijs = (RijbewijsEnum) combobx_Rijbewijs.SelectedItem;

            try {
                TypeVoertuig tv = new TypeVoertuig(gevondenType, gevondenRijbewijs);
                tvm.voegTypeVoertuigToe(tv);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }
    }
}
