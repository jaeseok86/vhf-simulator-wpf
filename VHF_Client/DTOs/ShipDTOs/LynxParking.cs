using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct LynxParking
    {
        public int nShipNo;
        [MarshalAs(UnmanagedType.I1)]
        public bool bParking;
    }
}
