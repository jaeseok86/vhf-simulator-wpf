using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct ST_TUG_WINCH_INFO
    {
        public double dLineLength;
        public float fLineAngle;
        public double dTension;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] dTugForce;
        public int nChockState;

        public int bOverload;
        public int bLowLevelAlarm;
    }
}
