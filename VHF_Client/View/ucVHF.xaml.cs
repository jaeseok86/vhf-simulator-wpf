using System.Windows.Controls;
using VHF_Client.ViewModel;

namespace VHF_Client.View
{
    /// <summary>
    /// ucVHF.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucVHF : UserControl
    {
        VHFViewModel viewModel = new VHFViewModel();

        public ucVHF()
        {
            InitializeComponent();

            DataContext = viewModel; // new VHFViewModel();

            //viewModel = DataContext as VHFViewModel;
        }

        public void CloseProcess()
        {
            viewModel?.CloseProcess();
        }

        public void StartSendVoice()
        {
            viewModel?.StartSendVoice();
        }

        public void StopSendVoice()
        {
            viewModel?.StopSendVoice();
        }

        public bool IsVoiceSendingByKeyboard()
        {
            if (viewModel != null)
            {
                return viewModel.IsVoiceSendingByKeyboard();
            }

            return false; // PTT 마이크 사용으로 처리됨
        }
    }
}
