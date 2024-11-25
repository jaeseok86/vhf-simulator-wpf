using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Client.Model.SubMenu
{
    public class DistressCallModel
    {
        public string Format = "";
        public string Address = "";
        public string Latitude = "";
        public string Longitude = "";
        public string LocalTime = "";

        private static DistressCallModel _instance;
        public static DistressCallModel Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new DistressCallModel();
                }
                return _instance;
            }
        }

        public DistressCallModel() { }
    }
}
