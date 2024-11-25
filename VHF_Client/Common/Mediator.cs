using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHF_Client.Model;

namespace VHF_Client.Common
{
    public class Mediator
    {
        public enum InputTextType
        {
            None = 0,
            address,
            latitude, 
            longitude,
            localTime,
            mmsi
        }

        public event Action<string> ChannelChangeAction;
        public event Action ShowMainScreenAction;
        //public event Action<string> WatchTypeChangeAction;
        public event Action<WatchType> WatchTypeChangeAction;
        public event Action<string> CannelModeChangeAction;
        public event Action<string> PowerChangeAction;

        public event Action<string> LatitudeChangeAction;
        public event Action<string> LongitudeChangeAction;
        public event Action<string> LoacaltimeChangeAction;

        public event Action<DscMessage> DscMessageSendAction;

        public event Action<bool> AutoPrintChangeAction;

        public event Action<InputTextType, string> DscTextboxFocusAction;

        public event Action<string> DscTextboxValueChangeAction;

        public event Action<string> MmsiChangeAction;

        public event Action<string> MmsiSendAction;

        public void NotifyMmsiSend(string mmsi)
        {
            MmsiSendAction?.Invoke(mmsi);
        }

        public void NotifyMmsiChanged(string mmsi)
        {
            MmsiChangeAction?.Invoke( mmsi);
        }

        public void NotifyDscTextboxValueChanged(string text)
        {
            DscTextboxValueChangeAction?.Invoke(text);
        }

        public void NotifyDscTextboxFocusChanged(InputTextType type, string text)
        {
            DscTextboxFocusAction?.Invoke(type, text);
        }

        public void NotifyAutoPrintChanged(bool isAutoPrint)
        {
            AutoPrintChangeAction?.Invoke(isAutoPrint);
        }

        public void NotifyDscMessageSend(DscMessage dscMessage)
        {
            DscMessageSendAction?.Invoke(dscMessage);
        }
                
        public void NotifyChannelChanged(string channel)
        {
            ChannelChangeAction?.Invoke(channel);
        }

        //public void NotifyWatchTypeChanged(string type)
        //{
        //    WatchTypeChangeAction?.Invoke(type);
        //}

        public void NotifyWatchTypeChanged(WatchType type)
        {
            WatchTypeChangeAction?.Invoke(type);
        }

        public void NotifyChannelModeChanged(string channelMode)
        {
            CannelModeChangeAction?.Invoke(channelMode);
        }

        public void NotifyPowerChanged(string power)
        {
            PowerChangeAction?.Invoke(power);
        }

        public void NotifyLatitudeChanged(string power)
        {
            LatitudeChangeAction?.Invoke(power);
        }

        public void NotifyLongitudeChanged(string power)
        {
            LongitudeChangeAction?.Invoke(power);
        }

        public void NotifyLoacaltimeChanged(string power)
        {
            LoacaltimeChangeAction?.Invoke(power);
        }

        public void NotifyShowMainScreen()
        {
            ShowMainScreenAction?.Invoke();
        }

        #region DSC Menu 화 DSC Sub View Event

        public event Action CloseSubViewAction;

        public void NotifySubviewClosed()
        {
            CloseSubViewAction?.Invoke();
        }

        #endregion

        private static Mediator _instance;

        private Mediator() { }

        public static Mediator instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Mediator();
                }
                return _instance;
            }
        }
    }
}
