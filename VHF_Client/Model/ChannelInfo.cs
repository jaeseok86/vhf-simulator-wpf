using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHF_Client.ViewModel;

namespace VHF_Client.Model
{
    public enum WatchType
    {
        Normal = 0,
        DualWatch,
        TripleWatch,
        AllWatch,
        TagWatch
    }

    public class ChannelInfo// : ViewModelBase
    {
        public string tempNewChannel = "";
        public string vhfChannel = "16"; // 시작시 16번 사용
        public string orgUsedChannel = ""; // Dual watch, Triple watch 또는 [16/9] 버튼 사용시 다시 현재 채널을 사용하기 위해 사용됨.
        public WatchType watchType = WatchType.Normal;
        public string watchTypeValue = "";
        public string channelMode = "ITU";

        private static ChannelInfo _instance;

        private ChannelInfo() { }

        public static ChannelInfo Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ChannelInfo();
                }
                return _instance;
            }
        }
    }
}
