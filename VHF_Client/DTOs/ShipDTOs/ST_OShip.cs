using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct ST_OShip
    {
        public uint m_dwStructSize;
        public double m_ToleranceRate;

        //= 시뮬레이션 환경 ----------------------------------------------------------------------------

        public uint m_dwNetworkSimId;
        public int m_lPacketId; //= long

        public int m_nIOSType;  //= {General, Azimuth, TugBoat, DynamicPosition }
        public ushort m_wSimMode;
        public ushort m_wSimSpeed;
        public ushort m_wOutputStep;

        public int m_bFinish;
        public int m_bRunning;

        public ulong m_timeStart;
        public double m_SimulationTime;
        public uint m_dwSystemTime;
        public double m_TimeStep;
        public uint m_dwTimeStep;

        public uint m_dwHostAddress;

        public int m_bTwinEngine;
        public int m_bBridgeControl;

        //= Motion Solver Parameter -------------------------------------------------------------------

        public ushort m_wOwnshipType;
        public ushort m_wEngineType;
        public ushort m_wMDevType;

        public double m_MaxAheadRPM;
        public double m_MaxAsternRPM;

        //= 시나리오 환경 ------------------------------------------------------------------------------

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szSimulationID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szHarborName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szShipName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szCurrentFile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public sbyte[] m_szWaveFile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
        public sbyte[] m_szBollardName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] m_szVisualScene;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] m_szSituationDisp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] m_szRadarLandmass;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public sbyte[] m_szOwnshipBow;

        public ushort m_wAnchorCount;
        public ushort m_wTrafficCount;
        public ushort m_wTugCount;
        public ushort m_wNoClients;
        public ushort m_wBollardCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ST_POINT_3D[] m_AnchorPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public ST_POINT_3D[] m_ChockPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public ST_POINT_3D[] m_BollardPos;

        public Pos2D m_OriginLatLon;

        //= Environment -------------------------------------------------------------------------------

        public int m_bInstructDepth;
        public int m_bInstructWind;
        public int m_bInstructWave;
        public int m_bInstructCurrent;
        public int m_bWindNoise;

        public double m_CmdWaterDepth;
        public double m_WaterDepth;

        public double m_CmdWindSpeed;
        public double m_WindSpeed;
        public double m_WindRelSpeed;

        public double m_CmdWindDir;
        public double m_WindDir;
        public double m_WindRelDir;

        public double m_CmdWaveHeight;
        public double m_WaveHeight;

        public double m_CmdWaveDir;
        public double m_WaveDirection;

        public double m_CmdCurrentSpeed;
        public double m_CurrentDrift;

        public double m_CmdCurrentDir;
        public double m_CurrentSet;

        public double m_CmdCurrentHeight;
        public double m_HeightTide;

        public double m_MagneticVar;
        public double m_GyroHeading;

        public double m_DayTimeSecond;
        public double m_VisibleRange;
        public ushort m_wVisibility;
        public byte m_byDayCondition;

        public int nWeatherParam;
        public int nCloudParam;
        public int nNightParam;

        public int m_bThunder;
        public int m_bBuzzer;
        public int nClashSound;

        //= OwnShip Information -----------------------------------------------------------------------

        public double m_OwnshipLength;
        public double m_OwnshipBeam;
        public double m_OwnshipDraft;
        public int m_bCPP;

        public ST_POINT_3D m_OwnshipPos;
        public Pos2D m_LatLon;
        public Pos2D m_GPSLatLon;

        public double m_OwnTurningRate;
        public double m_OwnshipHeading;
        public double m_OwnshipPitch;
        public double m_OwnshipRoll;
        public double m_OwnshipLatVel;
        public double m_OwnshipLongVel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_LatVel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_RudderCommand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_RudderValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_EngineCommand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_EngineValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_RPM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_Pitch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_BucketCommand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_BucketValue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public MLine[] m_MooringLine;

        public int m_bPositionFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public int[] m_bLightNav;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public int[] m_bLightSig1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] m_bLightSig2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public int[] bDeckLight;
        public int cLightIntensity;

        public ShipFlag m_stShipFlag;
        public ShipShape m_stShipShape;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bSpotLight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_SpotBearing;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_SpotElevation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_SpotWidth;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Pos2D[] m_AnchorPos;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[] m_wAnchStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_AnchLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_AnchorTotalLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bThrusterOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_ThrustCmd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_ThrusterThrust;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_dDP_ThrusterCmd; // DPS -> IOS -> MotionSlover
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_dDP_ThrusterRpm; // MotionSlover -> IOS -> DPS
        public int m_bDP_Auto_All;         // DPS -> IOS -> MotionSlover
        public int m_bDP_Auto_Surge;     // DPS -> IOS -> MotionSlover
        public int m_bDP_Auto_Sway;    // DPS -> IOS -> MotionSlover
        public int m_bDP_Auto_Yaw;       // DPS -> IOS -> MotionSlover
        public int m_nJoystick_x;           // DPS -> IOS -> MotionSlover
        public int m_nJoystick_y;           // DPS -> IOS -> MotionSlover
        public int m_nJoystick_z;           // DPS -> IOS -> MotionSlover

        public int m_bOwnshipWhistle;
        public int m_bAccident;

        public ST_AISINFO m_stAISInfo;                                              //= JJY 20180702

        //= Tugboat Simulation (Motion Solver) --------------------------------------------------------

        public ST_TUG_WINCH_CMD m_stTugWinchCmd;                            //= JJY 20180618
        public ST_TUG_WINCH_INFO m_stTugWinchInfo;                      //= JJY 20180618

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]    //= JJY 20180618
        public double[] dAccel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]    //= JJY 20180618
        public double[] dPSVelocity;

        //= Fail Alarm Information --------------------------------------------------------------------

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public int[] m_bAlarm;

        public int m_bFailRadar;
        public int m_bFailSteeringWheel;
        public int m_bFailSteeringWheel2;
        public int m_bFailSteerPumpMain;
        public int m_bFailSteerPumpCenter;
        public int m_bFailSteerPumpSub;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bFailEngine;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bFailTurbine;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bFailPitch;
        public int m_bFailGyroAbnormal;
        public int m_bFailGyroFail;
        public int m_bFailGPS;
        public int m_bFailLOG;
        public int m_bFailECHO;

        //= Engine Control Parameter ------------------------------------------------------------------

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_PitchTransv;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bDieselStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bDieselRun;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_DieselCmd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_DieselRPM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_DieselShaft;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bTurbineStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bTurbineRun;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_TurbineCmd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_TurbineRPM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_TurbineShaft;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_bThrusterExist;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public double[] m_ThrusterRPM;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] m_FuelTendency;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_iEngineSel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_iManeuverMode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bClutchDiesel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bClutchDieselAck;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bClutchTurbine;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bClutchTurbineAck;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] m_bSemiMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] m_bySemiDieselRPM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] m_bySemiLongPitch;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] m_bySemiTranPitch;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] m_byVSPEngSpeed;

        public double m_MaxDieselRPM;
        public double m_MaxTurbineRPM;

        //= 특수 기능 (3D) -----------------------------------------------------------------------------

        public int m_bLanding;
        public int m_bTakeoff;

        public int bShot;
        public int bSink;
        public int bLifeTube;
        public int bLifeBoat;
        public int bLifeFlare;
        public int bBoatFlare;
        public int bSonobuoy;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ShipGun[] stOwnShipGun;
        public LynxParking m_stLynxParking;

        public int lViewPoint;
        public int nViewShipNum;
        public int nZoomInOut;

        //= ETC ---------------------------------------------------------------------------------------

        public int m_bFinStabilizerOn;
        public ushort m_wNFUCommand;
        public byte m_bySteeringSel;

        public ushort m_wSteeringMode;
        public double m_ComdAutoCourse;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[] m_wZDir;
    }
}
