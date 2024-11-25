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

namespace VHF_Client.View.SubMenu
{
    /// <summary>
    /// PrintSetupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PrintSetupView : UserControl
    {
        Mediator mediator = Mediator.instance;

        public PrintSetupView()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            mediator.NotifySubviewClosed();
        }

        private void tgAutoPrint_Checked(object sender, RoutedEventArgs e)
        {
            mediator.NotifyAutoPrintChanged(true);
        }

        private void tgAutoPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            mediator?.NotifyAutoPrintChanged(false);
        }
    }
}
