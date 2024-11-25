using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Client.Services
{
    public class ChannelNameProvider
    {
        public ChannelNameProvider() { }

        public string GetChannelName(string channel)
        {
            // 추후 channel이 문자열이 표함될 수도 있음. (ex : 01A 등)
            // 기본 ITU Channel Name으로 임시 통일함

            string channelName = "";

            if( channel == "1" || channel == "2" || channel == "3" || channel == "4" || channel == "23" || 
                channel == "24" || channel == "25" || channel == "26" || channel == "27" || channel == "28" || 
                channel == "60" || channel == "64" || channel == "83" || channel == "84" || channel == "85" || 
                channel == "86")
            {
                channelName = "TELEPHONE";
            }
            else if (channel == "4" || channel == "7" || channel == "15" || channel == "18" || channel == "20" ||
                     channel == "21" || channel == "22" || channel == "61" || channel == "62" || channel == "63" ||
                     channel == "65" || channel == "66" || channel == "69" || channel == "71" || channel == "73" ||
                     channel == "74" || channel == "81" || channel == "82")
            {
                channelName = "PORT OPS";
            }
            else if (channel == "16")
            {
                channelName = "DISTRESS";
            }
            else if (channel == "70")
            {
                channelName = "DSC";
            }
            else if (channel == "19" || channel == "78" || channel == "79" || channel == "80")
            {
                channelName = "SHIP-SHORE";
            }
            else if (channel == "72" || channel == "77" || channel == "87" || channel == "88")
            {
                channelName = "SHIP-SHIP";
            }
            else if (channel == "5" || channel == "12" || channel == "14")
            {
                channelName = "PORT OPS/VTS";
            }
            else if (channel == "6")
            {
                channelName = "SAFETY";
            }
            else if (channel == "8" || channel == "10")
            {
                channelName = "COMMERCIAL";
            }
            else if (channel == "9")
            {
                channelName = "CALLING";
            }
            else if (channel == "11")
            {
                channelName = "VTS";
            }
            else if (channel == "13" || channel == "67")
            {
                channelName = "BRIDGE COM";
            }
            else if (channel == "17")
            {
                channelName = "SAR";
            }
            //else if (channel == "" || channel == "" || channel == "" || channel == "" || channel == "" ||
            //         channel == "" || channel == "" || channel == "" || channel == "" || channel == "")
            //{
            //    channelName = "";
            //}

            return channelName;
        }
    }
}
