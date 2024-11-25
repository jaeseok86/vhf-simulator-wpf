using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//++ 현재 사용하고 있지 않음.
namespace VHF_Client.ViewModel
{
    class DSCMenuViewModel : ViewModelBase
    {
        public Command DistressCallCommand { get; set; }
        public Command AllShipCallCommand { get; set; }
        public Command IndividualCallCommand { get; set; }

        public DSCMenuViewModel()
        {
            InitButtonCommand();
        }

        private void InitButtonCommand()
        {
            DistressCallCommand = new Command(ShowDistressCallView, CanExecute);
            AllShipCallCommand = new Command(ShowDistressCallView, CanExecute);

            IndividualCallCommand = new Command(ShowDistressCallView, CanExecute);
        }

        private void ShowDistressCallView(object obj)
        {
            string type = obj as string;
        }

        private bool CanExecute(object obj)
        {
            return true;
        }
    }
}
