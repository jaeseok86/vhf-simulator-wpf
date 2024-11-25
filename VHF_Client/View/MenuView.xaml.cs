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
using VHF_Client.ViewModel.SubMenu;

namespace VHF_Client.View
{
    /// <summary>
    /// MenuView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuView : UserControl
    {
        enum Viewtype
        {
            None = 0,
            PrintSetupView,
            MmsiSetView
        }

        Viewtype viewtype = Viewtype.None;
        Mediator mediator = Mediator.instance;

        PrintSetupView printSetupView = new PrintSetupView();
        DSCSetView DSCSetView = new DSCSetView();

        public MenuView()
        {
            InitializeComponent();

            //gridSubView.Children.Add(printSetupView);

            mediator.CloseSubViewAction += Mediator_CloseSubViewAction;
        }

        private void Mediator_CloseSubViewAction()
        {
            VisibleSubView(false);
        }

        private void btnPrintSetup_Click(object sender, RoutedEventArgs e)
        {
            if(viewtype  != Viewtype.PrintSetupView)
            {
                if (gridSubView.Children.Count == 0)
                {
                    gridSubView.Children.Add(printSetupView);
                }
                else if (gridSubView.Children.Count > 0)
                {
                    gridSubView.Children.RemoveAt(0);
                    gridSubView.Children.Add(printSetupView);
                }
            }

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

        private void btnDscSet_Click(object sender, RoutedEventArgs e)
        {
            if (viewtype != Viewtype.MmsiSetView)
            {
                if (gridSubView.Children.Count == 0)
                {
                    gridSubView.Children.Add(DSCSetView);
                }
                else if (gridSubView.Children.Count > 0)
                {
                    gridSubView.Children.RemoveAt(0);
                    gridSubView.Children.Add(DSCSetView);
                }
            }

            VisibleSubView(true);
        }
    }
}
