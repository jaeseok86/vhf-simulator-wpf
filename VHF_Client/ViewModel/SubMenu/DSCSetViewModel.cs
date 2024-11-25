using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VHF_Client.Common;
using VHF_Client.Model.SubMenu;

namespace VHF_Client.ViewModel.SubMenu
{

    public class DSCSetViewModel : ViewModelBase
    {
        Mediator mediator = Mediator.instance;

        DSCSetModel dscSetModel = DSCSetModel.Instance;

        public string MMSI
        {
            get => dscSetModel.mmsi;
            set => SetProperty(ref dscSetModel.mmsi, value);
        }

        public DSCSetViewModel()
        {
            mediator.MmsiChangeAction += Mediator_MmsiChangeAction;
        }

        private void Mediator_MmsiChangeAction(string mmsi)
        {
            MMSI = mmsi;
        }

        public void SetFocusTextInfo()
        {
            mediator.NotifyDscTextboxFocusChanged(Mediator.InputTextType.mmsi, MMSI);
        }

        public void SendMmsi()
        {
            mediator.NotifyMmsiSend(MMSI);
        }

        public void CloseView()
        {
            MMSI = string.Empty;

            mediator.NotifyDscTextboxFocusChanged(Mediator.InputTextType.None, "");

            mediator.NotifySubviewClosed();
        }
    }
}
