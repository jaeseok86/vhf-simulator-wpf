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
using VHF_Client.View.SubMenu;
using VHF_Client.ViewModel;

namespace VHF_Client.View
{
    /// <summary>
    /// DSCMenuView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DSCMenuView : UserControl
    {
        Mediator mediator = Mediator.instance;

        DistressCallView distressCallView = new DistressCallView();
        public DSCMenuView()
        {
            InitializeComponent();

            gridSubView.Children.Add(distressCallView);

            mediator.CloseSubViewAction += Mediator_CloseSubViewAction;
        }

        private void Mediator_CloseSubViewAction()
        {
            VisibleSubView(false);
        }

        private void btnDistressCAll_Click(object sender, RoutedEventArgs e)
        {
            distressCallView.SetDscType(DistressCallView.DSCType.DistressCall);
            VisibleSubView(true);
        }

        private void btnAllShipCall_Click(object sender, RoutedEventArgs e)
        {
            distressCallView.SetDscType(DistressCallView.DSCType.AllShipCall);
            VisibleSubView(true);
        }

        private void btnIndividualCall_Click(object sender, RoutedEventArgs e)
        {
            distressCallView.SetDscType(DistressCallView.DSCType.IndividualCall);
            VisibleSubView(true);
        }

        private void VisibleSubView(bool isVisible)
        {
            if (isVisible)
            {
                gridSubView.Visibility = Visibility.Visible;
            }
            else
            {
                gridSubView.Visibility = Visibility.Collapsed;
            }
        }
    }
}
