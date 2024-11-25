using System.Windows.Controls;
using VHF_Client.ViewModel;

namespace VHF_Client.View
{
    /// <summary>
    /// VHFView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VHFView : UserControl
    {
        VHFViewModel viewModel = new VHFViewModel();

        public VHFView()
        {
            InitializeComponent();

            DataContext = viewModel;
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
