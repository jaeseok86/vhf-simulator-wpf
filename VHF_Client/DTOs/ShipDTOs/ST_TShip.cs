using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct ST_TShip
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szShipName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szShipType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] m_szDBName;
        public ushort m_wShipType;

        public double m_Length;
        public double m_Beam;

        public byte m_byControlMode;
        public ST_POINT_3D m_TrafficPos;
        public Pos2D m_TShipLatLon;

        public double m_TrafficHeading;
        public double m_TrafficPitch;
        public double m_TrafficRoll;
        public double m_LatVel;
        public double m_LongVel;
        public double m_RudderAngle;
        public double m_TurningRate;

        //= TrafficShip 보급선 -------------------------------------------------------------------------

        public int m_bDispShip;
        public int m_bPositionFlag;
        public int m_bSupplyLine;
        public int m_bWhistle;
        public int m_bTowline;

        //= TrafficShip Light/Flag/ShipShape ----------------------------------------------------------

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public int[] m_bLightNav;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public int[] m_bLightSig1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] m_bLightSig2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public int[] bDeckLight;

        public ShipFlag stShipFlag;
        public ShipShape stShipShape;

        //= TrafficShip 예인선 -------------------------------------------------------------------------

        public double m_MaxTugForce;
        public int m_bTugEnable;
        public ushort m_wTugPosition;
        public double m_TugThrust;
        public double m_TugDirection;
        public ST_POINT_3D m_TowlinePoint;

        public ushort m_wTugCommand;
        public ushort m_wTugFlag;
        public int m_bTugCWDir;

        //= 특수 기능 (3D) -----------------------------------------------------------------------------

        public int bShot;
        public int bSink;
        public int bLifeTube;
        public int bLifeBoat;
        public int bLifeFlare;
        public int bBoatFlare;
        public int bSonobuoy;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ShipGun[] stTShipGun;
        public LynxParking m_stLynxParking;

        //= Tugboat Simulation --------------------------------------------------------------- 20180504

        public ushort m_wSimMode;
        public ST_TUG_WINCH_INFO m_stTugInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public double[] m_dTugForce;

        public ST_AISINFO m_stAISInfo;//= JJY 20180702
    }
}
