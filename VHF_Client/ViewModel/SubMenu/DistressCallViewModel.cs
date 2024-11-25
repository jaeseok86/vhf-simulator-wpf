using System.Security.Cryptography;
using VHF_Client.Common;
using VHF_Client.Model;
using VHF_Client.Model.SubMenu;
using static System.Net.Mime.MediaTypeNames;

namespace VHF_Client.ViewModel.SubMenu
{
    public class DistressCallViewModel : ViewModelBase
    {
        Mediator mediator = Mediator.instance;

        DistressCallModel distressCallModel = DistressCallModel.Instance;

        Mediator.InputTextType inputDscTextType = Mediator.InputTextType.None;

        bool isVisibleView = false;

        public string Format
        {
            get => distressCallModel.Format;
            set => SetProperty(ref distressCallModel.Format, value);
        }

        public string Address
        {
            get => distressCallModel.Address;
            set => SetProperty(ref distressCallModel.Address, value);
        }

        public string Latitude
        {
            get => distressCallModel.Latitude;
            set => SetProperty(ref distressCallModel.Latitude, value);
        }

        public string Longitude
        {
            get => distressCallModel.Longitude;
            set => SetProperty(ref distressCallModel.Longitude, value);
        }

        public string LocalTime
        {
            get => distressCallModel.LocalTime;
            set => SetProperty(ref distressCallModel.LocalTime, value);
        }

        public DistressCallViewModel()
        {
            mediator.LatitudeChangeAction += Mediator_LatitudeChangeAction;
            mediator.LongitudeChangeAction += Mediator_LongitudeChangeAction;
            mediator.LoacaltimeChangeAction += Mediator_LoacaltimeChangeAction;

            mediator.DscTextboxValueChangeAction += Mediator_DscTextboxValueChangeAction;
        }

        private void Mediator_DscTextboxValueChangeAction(string text)
        {
            switch (inputDscTextType)
            {
                case Mediator.InputTextType.address:
                    Address = text;
                    break;
                case Mediator.InputTextType.latitude:
                    Latitude = text;
                    break;
                case Mediator.InputTextType.longitude:
                    Longitude = text;
                    break;
                case Mediator.InputTextType.localTime:
                    LocalTime = text;
                    break;
                default:
                    break;
            }
        }

        private void Mediator_LatitudeChangeAction(string obj)
        {
            if (isVisibleView == false)
            {
                return;
            }

            if (Latitude != "")
            {
                return;
            }

            Latitude = obj.ToString();
        }

        private void Mediator_LongitudeChangeAction(string obj)
        {
            if (isVisibleView == false)
            {
                return;
            }

            if (Longitude != "")
            {
                return;
            }

            Longitude = obj.ToString();
        }

        private void Mediator_LoacaltimeChangeAction(string obj)
        {
            if (isVisibleView == false)
            {
                return;
            }

            if (LocalTime != "")
            {
                return;
            }

            LocalTime = obj.ToString();
        } 

        public void CloseView()
        {
            Address = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;
            LocalTime = string.Empty;

            mediator.NotifyDscTextboxFocusChanged(Mediator.InputTextType.None, "");

            mediator.NotifySubviewClosed();
        }

        public void SendDscMessage(string nature, string category)
        {
            DscMessage dscMessage = new DscMessage()
            {
                format = Format,
                nature = nature,
                address = Address,
                category = category,
                latitude = Latitude,
                longitude = Longitude,
                utcTime = LocalTime
            };

            mediator.NotifyDscMessageSend(dscMessage);
        }

        public void SetFormatType(string format)
        {
            Format = format;

            isVisibleView = true;
        }

        public void SetAddress(string address)
        {
            Address = address;
        }

        public void SetFocusTextInfo(Mediator.InputTextType type)
        {
            inputDscTextType = type;

            string text = "";
            
            switch(type)
            {
                case Mediator.InputTextType.address:
                    text = Address;
                    break;
                case Mediator.InputTextType.latitude:
                    text = Latitude;
                    break;
                case Mediator.InputTextType.longitude:
                    text = Longitude;
                    break;
                case Mediator.InputTextType.localTime:
                    text = LocalTime;
                    break;
                default:
                    break;
            }

            mediator.NotifyDscTextboxFocusChanged(type, text);
        }
    }
}
