using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Client.Model
{
    public class DscMessage // DSC Message Structure
    {
        public string format { get; set; } // DISTRESS / ALL SHIP / INDIVIDUAL
        public string nature { get; set; } //UNDEFINED, FIRE, FLOODING, COLLISION, etc...
        public string address { get; set; } // Target 이 되는 Individual MMSI (MMSI 번호는 9자리 숫자로 구성)
        public string category { get; set; } // ROUTINE, SAFETY, URGENCY
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string utcTime { get; set; } // UTC Time 사용유무는 추가 확인 필요


        //Distress와 Others 모두 채널 70을 사용함. 법적 기술적 표준 추가확인 필요

        //public string workChR { get; set; } // (ALL SHIP / INDIVIDUAL인 경우 설정 가능) / DISTRESS인 경우 Ch 70 고정
        //public string workChT { get; set; }
    }
}
