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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VHF_Client.Common;
using VHF_Client.ViewModel.SubMenu;

namespace VHF_Client.View.SubMenu
{
    /// <summary>
    /// DSCSetView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DSCSetView : UserControl
    {
        Mediator mediator = Mediator.instance;

        DSCSetViewModel DSCSetViewModel = new DSCSetViewModel();

        public DSCSetView()
        {
            InitializeComponent();
            DataContext = DSCSetViewModel;
        }

        private void btnSetMmsi_Click(object sender, RoutedEventArgs e)
        {
            DSCSetViewModel?.SendMmsi();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            DSCSetViewModel?.CloseView();
        }

        private void txtMmsi_GotFocus(object sender, RoutedEventArgs e)
        {
            DSCSetViewModel?.SetFocusTextInfo();
        }
    }
}
