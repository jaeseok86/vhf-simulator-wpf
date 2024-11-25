using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VHF_Client.Common;
using VHF_Client.ViewModel.SubMenu;

namespace VHF_Client.View.SubMenu
{
    /// <summary>
    /// DistressCallView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DistressCallView : UserControl
    {
        public enum DSCType
        {
            None = 0,
            DistressCall,
            AllShipCall,
            IndividualCall
        }

        DSCType dscType = DSCType.None;

        bool isVisibleNow = false;

        DistressCallViewModel distressCallViewModel = new DistressCallViewModel();

        public DistressCallView()
        {
            InitializeComponent();

            DataContext = distressCallViewModel;
        }

        public void SetDscType(DSCType type)
        {
            dscType = type;

            VisibleInfomations();
        }

        private void VisibleInfomations()
        {
            //화면에 표시된 정보는 각 Type 별로 사용되는 경우가 다름

            tbNature.Foreground = Brushes.Black;
            cbNature.IsEnabled = true;

            tbAddress.Foreground = Brushes.Black;
            txtAddress.IsEnabled = true;

            tbCategory.Foreground = Brushes.Black;
            cbCategory.IsEnabled = true;

            btnTempMmis1.IsEnabled = true;
            btnTempMmis2.IsEnabled = true;
            btnTempMmis3.IsEnabled = true;
            btnTempMmisErr.IsEnabled = true;

            string format = "";

            if (dscType == DSCType.DistressCall)
            {
                format = "DISTRESS";

                tbAddress.Foreground = Brushes.LightGray;
                txtAddress.IsEnabled = false;

                tbCategory.Foreground = Brushes.LightGray;
                cbCategory.IsEnabled = false;

                btnTempMmis1.IsEnabled = false;
                btnTempMmis2.IsEnabled = false;
                btnTempMmis3.IsEnabled = false;
                btnTempMmisErr.IsEnabled = false;
            }
            else if (dscType == DSCType.AllShipCall)
            {
                format = "ALL SHIP";

                tbNature.Foreground = Brushes.LightGray;
                cbNature.IsEnabled = false;

                tbAddress.Foreground = Brushes.LightGray;
                txtAddress.IsEnabled = false;

                btnTempMmis1.IsEnabled = false;
                btnTempMmis2.IsEnabled = false;
                btnTempMmis3.IsEnabled = false;
                btnTempMmisErr.IsEnabled = false;
            }
            else if (dscType == DSCType.IndividualCall)
            {
                format = "INDIVIDUAL";

                tbNature.Foreground = Brushes.LightGray;
                cbNature.IsEnabled = false;
            }

            distressCallViewModel?.SetFormatType(format);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            CloseView();
        }

        private void CloseView()
        {
            cbNature.SelectedIndex = -1;
            cbCategory.SelectedIndex = -1;

            distressCallViewModel?.CloseView();
        }

        private void btnTransmit_Click(object sender, RoutedEventArgs e)
        {
            string nature = "";
            if (cbNature.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cbNature.SelectedItem;
                nature = selectedItem.Content.ToString();
            }

            string category = "";
            if (cbCategory.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cbCategory.SelectedItem;
                category = selectedItem.Content.ToString();
            }

            distressCallViewModel?.SendDscMessage(nature, category);

            CloseView();
        }

        private void btnTempMmis1_Click(object sender, RoutedEventArgs e)
        {
            //++ 임시로 버튼을 통한 MMSI 입력
            distressCallViewModel?.SetAddress("111111111");
        }

        private void btnTempMmis2_Click(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetAddress("222222222");
        }

        private void btnTempMmis3_Click(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetAddress("333333333");
        }

        private void btnTempMmisErr_Click(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetAddress("440999999");
        }

        private void txtAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetFocusTextInfo(Mediator.InputTextType.address);
        }

        private void txtLat_GotFocus(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetFocusTextInfo(Mediator.InputTextType.latitude);
        }

        private void txtLong_GotFocus(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetFocusTextInfo(Mediator.InputTextType.longitude);
        }

        private void txtTime_GotFocus(object sender, RoutedEventArgs e)
        {
            distressCallViewModel?.SetFocusTextInfo(Mediator.InputTextType.localTime);
        }
    }
}
