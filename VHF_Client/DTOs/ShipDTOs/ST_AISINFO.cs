using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct ST_AISINFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public sbyte[] m_szDestination;
        public int m_nMMSI;
        public int m_nIMONo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public sbyte[] m_szCallSign;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public sbyte[] m_szShipType;
        public int m_nDraft;
        public int m_bEnable;
    }
}
