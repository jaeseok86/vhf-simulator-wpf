using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VHF_Client.ViewModel;

namespace VHF_Client.View
{
    /// <summary>
    /// VolumeControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VolumeControlView : UserControl
    {
        VolumeControlViewModel VolumeControlViewModel = new VolumeControlViewModel();
        public VolumeControlView()
        {
            InitializeComponent();

            DataContext = VolumeControlViewModel;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeControlViewModel?.ControlVolume((float)VolumeSlider.Value);
        }
    }
}
