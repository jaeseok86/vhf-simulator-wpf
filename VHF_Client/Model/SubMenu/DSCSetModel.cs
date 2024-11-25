using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Client.Model.SubMenu
{
    public class DSCSetModel
    {
        public string mmsi = "";

        private static DSCSetModel _instance;
        public static DSCSetModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DSCSetModel();
                }
                return _instance;
            }
        }

        public DSCSetModel() { }
    }
}
