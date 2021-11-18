using FleetMgmt_Business.Enums;
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
    /// Interaction logic for BrandstofSelecteren.xaml
    /// </summary>
    public partial class BrandstofSelecteren : Window {

        public List<TankkaartBrandstof> brandstoffen { get; set; }
        public BrandstofSelecteren() {
            InitializeComponent();

            lstbx_Brandstoffen.ItemsSource = Enum.GetValues(typeof(TankkaartBrandstof));
        }

        private void btn_Selecteer_Click(object sender, RoutedEventArgs e) {
            if(lstbx_Brandstoffen.SelectedItems.Count < 1) {
                MessageBox.Show("U moet minstens 1 item selecteren!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } else {
                this.brandstoffen = lstbx_Brandstoffen.SelectedItems.Cast<TankkaartBrandstof>().ToList();
                DialogResult = true;
                Close();
            }
        }
    }
}
