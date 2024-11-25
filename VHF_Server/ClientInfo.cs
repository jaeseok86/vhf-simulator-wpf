using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VHF_Server
{
    public class ClientInfo
    {
        public IPEndPoint EndPoint { get; set; }
        public string SelectedChannel { get; set; }

        public string MMSI { get; set; }
    }
}
