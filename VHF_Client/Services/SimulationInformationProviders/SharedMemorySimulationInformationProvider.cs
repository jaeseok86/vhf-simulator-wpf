using STRRadar.DTOs;
using STRRadar.DTOs.ShipDTOs;
using STRRadar.Models;
using STRRadar.Models.Datum;
using STRRadar.Models.Racons;
using STRRadar.Models.RadarComponents;
using STRRadar.Models.Ships;
using STRRadar.Models.Signals;
using STRRadar.Services.SharedMemories;
using STRRadar.Services.TimeZone;
using STRRadar.Services.Udp;
using STRRadar.Utilities;
using STRRadar.ViewModels.ShipDataViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using static STRRadar.Models.Signals.Signal;

namespace STRRadar.Services.SimulationInformationProviders
{
    public class SimulationEventArgs : EventArgs
    {
        public SimulationInformation SimulationInformation { get; }

        public SimulationEventArgs(SimulationInformation simulationInformation) 
        {
            SimulationInformation = simulationInformation;
        }
    }

    public class SharedMemorySimulationInformationProvider : ISimulationInformationProvider
    {
        private readonly SharedMemoryService _sharedMemoryService;
        private readonly TimeZoneLookupService _timeZoneLookupService;
        private readonly IUdpService _sartUdpService;
        private readonly RaconFileRepository _raconFileRepository;
        private readonly List<ISignal> _sarts;
        private readonly DispatcherTimer _simulationTimer;

        private SimulationInformation? _simulationInformation;
        private SimulationStatus _simulationStatus;
        private int _packetId;
        private bool _isSartActive;

        public event EventHandler<SimulationEventArgs>? SimulationInitialized;
        public event EventHandler<SimulationEventArgs>? SimulationRunning;
        public event EventHandler<SimulationEventArgs>? SimulationPaused;
        public event EventHandler<SimulationEventArgs>? SimulationTerminated; 

        static ShipDataViewModel shipDataViewModel = new ShipDataViewModel();

        public SharedMemorySimulationInformationProvider(SharedMemoryService sharedMemoryService, TimeZoneLookupService timeZoneLookupService, IUdpService sartUdpService, RaconFileRepository raconFileRepository)
        {
            _sharedMemoryService = sharedMemoryService;
            _timeZoneLookupService = timeZoneLookupService;
            _sartUdpService = sartUdpService;
            _raconFileRepository = raconFileRepository;
            _sartUdpService.DataReceived += OnSartDataRecieved;
            _sartUdpService.Listen();
            _sarts = [];

            _simulationTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(16),
                DispatcherPriority.Normal,
                FetchSimulationInformation, 
                Dispatcher.CurrentDispatcher);
            _simulationInformation = null;
            _packetId = -8;
        }

        public void FetchSimulationInformation(object? sender, EventArgs e)
        {
            if (!_sharedMemoryService.HasData<ST_OShip>("STR_BCAST_DATA"))
            {
                if (_simulationStatus != SimulationStatus.StandBy)
                {
                    _packetId = -8;
                    _simulationStatus = SimulationStatus.StandBy;
                    OnSimulationTerminated(_simulationInformation);
                }

                _simulationInformation = null;
                return;
            }

            ST_OShip simulationInformation = _sharedMemoryService.ReadData<ST_OShip>("STR_BCAST_DATA");
            IEnumerable<int> trafficShipIndicies = Enumerable.Range(0, simulationInformation.m_wTrafficCount);
            IEnumerable<ST_TShip> trafficShipInformation
                = trafficShipIndicies.Select((index) =>
                {
                    return _sharedMemoryService.ReadData<ST_TShip>($"STR_TSHIP_DATA_#{index}");
                });

            int currentPacketId = simulationInformation.m_lPacketId;
            List<Racon> racons = _raconFileRepository.GetRacons();

            #region Racon Test
            //List<Racon> racons = [];

            //for (int i = 0; i < 360; i += 10)
            //{
            //    (double rotateX, double rotateY) = Geometry.RotateMatrix2D(0, 0.01, -i);

            //    racons.Add(new Racon(35.11175615 + rotateY, 128.63687263 + rotateX, (MorseCode)(i == 0 ? 0 : i / 10)));
            //}
            #endregion

            var localTime = _timeZoneLookupService.GetLocalTime(
                simulationInformation.m_LatLon.dX,
                simulationInformation.m_LatLon.dY,
                (long)simulationInformation.m_SimulationTime);

            _simulationInformation = ToSimulationInformation(
                _simulationStatus,
                simulationInformation,
                trafficShipInformation,
                racons,
                _sarts,
                _isSartActive,
                localTime);

            if (_packetId == -8 && currentPacketId == -8)
            {
                _simulationStatus = SimulationStatus.StandBy;
            }
            else if (_packetId == -8 && currentPacketId == 0)
            {
                _simulationStatus = SimulationStatus.Initialize;
                OnSimulationInitialized(_simulationInformation);
            }
            else if (_packetId != -8 && currentPacketId > 0)
            {
                _simulationStatus = SimulationStatus.Running;
                OnSimulationRunning(_simulationInformation);
            }
            else if (_packetId == -2 && currentPacketId == -2)
            {
                _simulationStatus = SimulationStatus.Pause;
                OnSimulationPaused(_simulationInformation);
            }
            else if (currentPacketId == -8)
            {
                _simulationStatus = SimulationStatus.StandBy;
                OnSimulationTerminated(_simulationInformation);
            }   

            _packetId = currentPacketId;   
        }

        public SimulationInformation? GetSimulationInformation()
        {
            return _simulationInformation;
            //if (!_sharedMemoryService.HasData<ST_OShip>("STR_BCAST_DATA"))
            //{
            //    return null;
            //}

            //ST_OShip simulationInformation = _sharedMemoryService.ReadData<ST_OShip>("STR_BCAST_DATA");
            //IEnumerable<int> trafficShipIndicies = Enumerable.Range(0, simulationInformation.m_wTrafficCount);
            //IEnumerable<ST_TShip> trafficShipInformation
            //    = trafficShipIndicies.Select((index) =>
            //    {
            //        return _sharedMemoryService.ReadData<ST_TShip>($"STR_TSHIP_DATA_#{index}");
            //    });

            //int currentPacketId = simulationInformation.m_lPacketId;

            //if (_simulationStatus == SimulationStatus.StandBy)
            //{
            //    if (_packetId == 0 && currentPacketId == -2)
            //    {
            //        _simulationStatus = SimulationStatus.Initialize;
            //    }

            //    if (_packetId == -2 && currentPacketId == 0)
            //    {
            //        _isRunning = true;
            //    }
            //}
            //else
            //{
            //    if (_isRunning)
            //    {
            //        _simulationStatus = SimulationStatus.Running;
            //    }

            //    if (_packetId == -2 && currentPacketId == 0)
            //    {
            //        _simulationStatus = SimulationStatus.Running;
            //    }
            //    else if (_packetId > 0 && currentPacketId == -2)
            //    {
            //        _simulationStatus = SimulationStatus.Puase;
            //    }
            //    else if (_packetId == -2 && currentPacketId > 0)
            //    {
            //        _simulationStatus = SimulationStatus.Running;
            //    }
            //    else if (currentPacketId == -8)
            //    {
            //        _simulationStatus = SimulationStatus.Terminate;
            //        _isRunning = false;
            //        _packetId = -8;
            //    }
            //    else if (currentPacketId != -2 && _packetId == currentPacketId)
            //    {
            //        //_counter++;

            //        //if (_counter == 5)
            //        //{
            //        //    _simulationStatus = SimulationStatus.Terminate;
            //        //    _isRunning = false;
            //        //    _packetId = -8;
            //        //}
            //    }
            //    else
            //    {
            //        _counter = 0;
            //        _simulationStatus = SimulationStatus.Running;
            //    }
            //}

            //_packetId = currentPacketId;

            //List<Racon> racons = _raconFileRepository.GetRacons();
            ////List<Racon> racons = [];

            ////for (int i = 0; i < 360; i += 10)
            ////{
            ////    (double rotateX, double rotateY) = Geometry.RotateMatrix2D(0, 0.01, -i);

            ////    racons.Add(new Racon(35.11175615 + rotateY, 128.63687263 + rotateX, (MorseCode)(i == 0 ? 0 : i / 10)));
            ////}

            //DateTime localTime = _timeZoneLookupService.GetLocalTime(simulationInformation.m_LatLon.dX, simulationInformation.m_LatLon.dY);

            //return ToSimulationInformation(_simulationStatus, simulationInformation, trafficShipInformation, racons, _sarts, _isSartActive, localTime);

            


            //IShip ownShip = new Ship
            //{
            //    Id = 0,
            //    Beam = 30,
            //    Length = 200,
            //    Heading = 30,
            //    COG = 10,
            //    SOG = 10,
            //    ROT = 16.6,
            //    STW = 8,
            //    // 진해 기준
            //    Latitude = 35.11175615 + _y * 6,
            //    Longitude = 128.63687263 + _x * 6,
            //    //Latitude = 35.11175615,
            //    //Longitude = 128.63687263,
            //    //Latitude = 39.631596,
            //    //Longitude = 124.386669,
            //    //// 울산 기준
            //    //Latitude = 35.379221,
            //    //Longitude = 129.3649448,
            //    //// 부산 기준
            //    //Latitude = 35.08874068,
            //    //Longitude = 129.07945957
            //    //// 인천 기준
            //    //Latitude = 37.1288798,
            //    //Longitude = 126.32398151
            //    //// 목포 기준
            //    //Latitude = 34.78001985,
            //    //Longitude = 126.38432812
            //    //// 안마도 기준
            //    //Latitude = 35.31073460,
            //    //Longitude = 126.13987745
            //    //// 제주 기준
            //    //Latitude = 33.53928513,
            //    //Longitude = 126.55244344
            //    //// 포항 기준
            //    //Latitude = 36.08147079,
            //    //Longitude = 129.50427766
            //    //// 뉴욕 기준
            //    //Latitude = 40.57378630,
            //    //Longitude = -74.03288116
            //    //// 시드니 기준
            //    //Latitude = -33.85406902,
            //    //Longitude = 151.25294621
            //};

            //List<AISInformation> AISInformationList = [
            //    new AISInformation
            //    {
            //        IMO = 9397568,
            //        MMSI = 441000000,
            //        CallSign = "D7RF",
            //        Type = ShipType.Cargo,
            //        Name = "Hanjin Shanghai",
            //        Length = 300,
            //        Breadth = 40,
            //        Destination = "Busan",
            //        ETA = new DateTime(2024, 7, 30, 12, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 9263113,
            //        MMSI = 441010012,
            //        CallSign = "D7FY",
            //        Type = ShipType.Passenger,
            //        Name = "Panstar Dream",
            //        Length = 199,
            //        Breadth = 28,
            //        Destination = "Jeju",
            //        ETA = new DateTime(2024, 7, 28, 9, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 8900114,
            //        MMSI = 440123456,
            //        CallSign = "D7BX",
            //        Type = ShipType.Fishing,
            //        Name = "Haeyang 5",
            //        Length = 50,
            //        Breadth = 10,
            //        Destination = "East Sea",
            //        ETA = new DateTime(2024, 7, 26, 5, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 9731914,
            //        MMSI = 440112233,
            //        CallSign = "D7ML",
            //        Type = ShipType.Tanker,
            //        Name = "SK Poseidon",
            //        Length = 250,
            //        Breadth = 44,
            //        Destination = "Ulsan",
            //        ETA = new DateTime(2024, 7, 25, 14, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 9111111,
            //        MMSI = 440011223,
            //        CallSign = "D7BZ",
            //        Type = ShipType.Cargo,
            //        Name = "Busan Trader",
            //        Length = 180,
            //        Breadth = 25,
            //        Destination = "Incheon",
            //        ETA = new DateTime(2024, 7, 27, 8, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 9111821,
            //        MMSI = 440011244,
            //        CallSign = "D7BZ",
            //        Type = ShipType.AISSart,
            //        Name = "Life Boat",
            //        Length = 10,
            //        Breadth = 4,
            //        Destination = "Incheon",
            //        ETA = new DateTime(2024, 7, 29, 16, 0, 0)
            //    },
            //    new AISInformation
            //    {
            //        IMO = 9731918,
            //        MMSI = 440242286,
            //        CallSign = "D7UA",
            //        Type = ShipType.Tanker,
            //        Name = "Hanhwa Ocean",
            //        Length = 300,
            //        Breadth = 65,
            //        Destination = "Pohang",
            //        ETA = new DateTime(2024, 7, 26, 22, 0, 0)
            //    },
            //];

            //var tship1 = new Ship
            //{
            //    Id = 1,
            //    Beam = 40,
            //    Length = 300,
            //    Heading = 0,
            //    COG = -1,
            //    ROT = 0,
            //    SOG = 4,
            //    Latitude = 35.114953 + _y * 4,
            //    Longitude = 128.632050,
            //    AISInformation = AISInformationList[0]
            //};
            //var tship2 = new Ship
            //{
            //    Id = 2,
            //    Beam = 28,
            //    Length = 199,
            //    Heading = 90,
            //    COG = 60,
            //    ROT = -10,
            //    SOG = 6,
            //    Latitude = 35.114953 + _y * 2,
            //    Longitude = 128.6342 + _x * 4,
            //    AISInformation = AISInformationList[1]
            //};
            //var tship3 = new Ship
            //{
            //    Id = 3,
            //    Beam = 10,
            //    Length = 50,
            //    Heading = 180,
            //    COG = 210,
            //    ROT = 10,
            //    SOG = 8,
            //    Latitude = 35.113745 - _y * 5,
            //    Longitude = 128.640096 - _x * 3,
            //    AISInformation = AISInformationList[2]
            //};
            //var tship4 = new Ship
            //{
            //    Id = 4,
            //    Beam = 44,
            //    Length = 250,
            //    Heading = 270,
            //    COG = 290,
            //    ROT = 10,
            //    SOG = 10,
            //    Latitude = 35.109632 + _y * 3,
            //    Longitude = 128.621312 - _x * 6,
            //    AISInformation = AISInformationList[3]
            //};
            //var tship5 = new Ship
            //{
            //    Id = 5,
            //    Beam = 25,
            //    Length = 180,
            //    Heading = 60,
            //    COG = 50,
            //    ROT = -5,
            //    SOG = 12,
            //    Latitude = 35.109632 - _y * 6,
            //    Longitude = 128.65134 + _x * 6,
            //    AISInformation = AISInformationList[4]
            //};
            //var tship6 = new Ship
            //{
            //    Id = 6,
            //    Beam = 4,
            //    Length = 10,
            //    Heading = 60,
            //    COG = 50,
            //    ROT = -5,
            //    SOG = 12,
            //    Latitude = 35.113745 - _y * 6,
            //    Longitude = 128.65134 + _x * 6,
            //    AISInformation = AISInformationList[5]
            //};
            //var tship7 = new Ship
            //{
            //    Id = 7,
            //    Beam = 25,
            //    Length = 180,
            //    Heading = 60,
            //    COG = 300,
            //    ROT = 20,
            //    SOG = 6,
            //    Latitude = 35.109632 - _y * 6,
            //    Longitude = 128.632050 + _x * 6,
            //    AISInformation = AISInformationList[6]
            //};
            //List<IShip> trafficShips = [tship1, tship2, tship3, tship4, tship5, tship6, tship7];
            ////DateTime localTime = _timeZoneLookupService.GetLocalTime(ownShip.Latitude, ownShip.Longitude);

            //_x += 0.000001;
            //_y += 0.000001;
            //_time += TimeSpan.FromMilliseconds(16);

            //if (_time.TotalSeconds >= 5 && _time.TotalSeconds < 10)
            //{
            //    trafficShips.Remove(tship3);
            //    trafficShips.Remove(tship4);
            //}
            //else if (_time.TotalSeconds >= 10 && _time.TotalSeconds < 15)
            //{
            //    trafficShips.Remove(tship5);
            //}
            //else if (_time.TotalSeconds >= 15 && _time.TotalSeconds < 20)
            //{
            //    trafficShips.Remove(tship1);
            //    trafficShips.Remove(tship6);
            //    trafficShips.Remove(tship7);
            //}
            //else if (_time.TotalSeconds >= 20 && _time.TotalSeconds < 25)
            //{
            //    trafficShips.Remove(tship2);
            //}

            //List<SystemError> systemErrors = [];

            ////if (_time.TotalSeconds >= 25)
            ////{
            ////    systemErrors.Add(SystemError.GPS);
            ////    systemErrors.Add(SystemError.Gyro);
            ////    systemErrors.Add(SystemError.DopplerLog);
            ////}
            ////else if (_time.TotalSeconds >= 20)
            ////{
            ////    systemErrors.Add(SystemError.Radar);
            ////}
            ////else if (_time.TotalSeconds >= 15)
            ////{
            ////    systemErrors.Add(SystemError.DopplerLog);
            ////}
            ////else if (_time.TotalSeconds >= 10)
            ////{
            ////    systemErrors.Add(SystemError.Gyro);
            ////}
            ////else if (_time.TotalSeconds >= 5)
            ////{
            ////    systemErrors.Add(SystemError.GPS);
            ////}

            //return new SimulationInformation(_simulationStatus, "Jinhae", ownShip, trafficShips, racons, _sarts, _isSartActive, 0, 0, localTime, systemErrors);
        }

        private static SimulationInformation ToSimulationInformation(
            SimulationStatus simulationStatus,
            ST_OShip simulationInformation,
            IEnumerable<ST_TShip> trafficShipInformations,
            IEnumerable<Racon> racons,
            IEnumerable<ISignal> sarts,
            bool isSartActive,
            DateTime localTime)
        {
            IShip ownShip = new Ship()
            {
                Id = 0,
                Name = SByteToString(simulationInformation.m_szShipName),
                Length = simulationInformation.m_OwnshipLength,
                Beam = simulationInformation.m_OwnshipBeam,
                Draft = simulationInformation.m_OwnshipDraft,
                Latitude = simulationInformation.m_LatLon.dX,
                Longitude = simulationInformation.m_LatLon.dY,
                Easting = simulationInformation.m_OwnshipPos.dX,
                Northing = simulationInformation.m_OwnshipPos.dY,
                Heading = simulationInformation.m_OwnshipHeading,
                Roll = simulationInformation.m_OwnshipRoll,
                Pitch = simulationInformation.m_OwnshipPitch,
                COG = CalculateCOG(simulationInformation.m_OwnshipLatVel, simulationInformation.m_OwnshipLongVel, simulationInformation.m_OwnshipHeading),
                SOG = CalculateSOG(simulationInformation.m_OwnshipLatVel, simulationInformation.m_OwnshipLongVel),
                ROT = simulationInformation.m_OwnTurningRate,
                STW = CalculateSTW(simulationInformation.m_OwnshipHeading, simulationInformation.m_CurrentSet, simulationInformation.m_OwnshipLongVel, simulationInformation.m_CurrentDrift),
                LateralVelocity = simulationInformation.m_OwnshipLatVel,
                LongitudinalVelocity = simulationInformation.m_OwnshipLongVel,
            };

            List<IShip> trafficShips = new List<IShip>();
            int trafficShipId = 1;

            foreach (ST_TShip trafficShipInformation in trafficShipInformations)
            {
                Ship trafficShip = new Ship()
                {
                    Id = trafficShipId++,
                    Name = SByteToString(trafficShipInformation.m_szShipName),
                    Length = trafficShipInformation.m_Length,
                    Beam = trafficShipInformation.m_Beam,
                    Draft = trafficShipInformation.m_stAISInfo.m_nDraft,
                    Latitude = trafficShipInformation.m_TShipLatLon.dX,
                    Longitude = trafficShipInformation.m_TShipLatLon.dY,
                    Easting = trafficShipInformation.m_TrafficPos.dX,
                    Northing = trafficShipInformation.m_TrafficPos.dY,
                    Heading = trafficShipInformation.m_TrafficHeading,
                    Roll = trafficShipInformation.m_TrafficRoll,
                    Pitch = trafficShipInformation.m_TrafficPitch,
                    COG = CalculateCOG(trafficShipInformation.m_LatVel, trafficShipInformation.m_LongVel, trafficShipInformation.m_TrafficHeading),
                    SOG = CalculateSOG(trafficShipInformation.m_LatVel, trafficShipInformation.m_LongVel),
                    ROT = trafficShipInformation.m_TurningRate,
                    LateralVelocity = trafficShipInformation.m_LatVel,
                    LongitudinalVelocity = trafficShipInformation.m_LongVel,
                    AISInformation = new AISInformation
                    {
                        IMO = trafficShipInformation.m_stAISInfo.m_nIMONo,
                        MMSI = trafficShipInformation.m_stAISInfo.m_nMMSI,
                        CallSign = SByteToString(trafficShipInformation.m_stAISInfo.m_szCallSign),
                        //Type = SByteToString(trafficShipInformation.m_stAISInfo.m_szShipType),
                        Type = ShipType.Cargo,
                        Name = SByteToString(trafficShipInformation.m_szShipName),
                        Length = trafficShipInformation.m_Length,
                        Breadth = trafficShipInformation.m_Beam,
                        Destination = SByteToString(trafficShipInformation.m_stAISInfo.m_szDestination),
                        ETA = DateTime.UtcNow
                    },
                };

                trafficShips.Add(trafficShip);
            }

            shipDataViewModel.AddShipInfo(ownShip, trafficShips);

            double waveHeight = Geometry.ConvertToPercentage(simulationInformation.m_WaveHeight, 0, 9);
            double rainFall = Geometry.ConvertToPercentage(simulationInformation.nWeatherParam < 1 ? 1 : simulationInformation.nWeatherParam, 1, 8);
            string harbor = SByteToString(simulationInformation.m_szRadarLandmass);
            List<SystemError> systemErrors = [];

            if (simulationInformation.m_bFailRadar != 0) 
            {
                systemErrors.Add(SystemError.Radar);
            }
            if (simulationInformation.m_bFailGPS != 0)
            {
                systemErrors.Add(SystemError.GPS);
            }
            if (simulationInformation.m_bFailGyroFail != 0)
            {
                systemErrors.Add(SystemError.Gyro);
            }
            if (simulationInformation.m_bFailLOG != 0)
            {
                systemErrors.Add(SystemError.DopplerLog);
            }

            return new SimulationInformation(simulationStatus, harbor, ownShip, trafficShips, racons, sarts, isSartActive, waveHeight, rainFall, localTime, systemErrors);
        }

        private static string SByteToString(sbyte[] sbytes)
        {
            if (sbytes == null)
            {
                return string.Empty;
            }

            return sbytes.Aggregate("", (current, value)
                => current + (value > 0 ? Convert.ToChar(value) : ""));
        }

        private static double CalculateCOG(double lateralVelocity, double longitudinalVelocity, double heading)
        {
            if (Math.Abs(longitudinalVelocity) < 0.002)
            {
                return 0;
            }

            double radian = Math.Atan2(lateralVelocity, longitudinalVelocity);
            double degree = Geometry.RadianToDegree(radian);
            double cog = heading + degree;

            while (cog < 0.0)
            {
                cog += 360.0;
            }

            while (cog > 360.0)
            {
                cog -= 360.0;
            }

            return cog;
        }

        private static double CalculateSOG(double lateralVelocity, double longitudinalVelocity)
        {
            double sog = Math.Sqrt(Math.Pow(lateralVelocity, 2) + Math.Pow(longitudinalVelocity, 2));

            return GeographicCalculator.MpsToKnots(sog);
        }

        private static double CalculateSTW(double heading, double current, double longitudinalVelocity, double currentDrift)
        {
            double degree = heading - current;
            double stw = longitudinalVelocity - currentDrift * Math.Cos(Geometry.DegreeToRadian(degree));

            return GeographicCalculator.MpsToKnots(stw);
        }

        private void OnSartDataRecieved(object? sender, DataReceivedEventArgs e)
        {
            BridgeIOS_SART sartDTOs = Marshaling.ConvertByteArrayToStructure<BridgeIOS_SART>(e.Data);

            bool isSartActive = sartDTOs.bSart == 1;

            if (!isSartActive)
            {
                _sarts.Clear();
                _isSartActive = isSartActive;

                return;
            }

            ISignal sartSignal = new SignalBuilder()
                .SetEasting(-2000)
                .SetNorthing(3000)
                .Build();

            //ISignal sartSignal = new SignalBuilder()
            //    .SetEasting(sartDTOs.dPositionX)
            //    .SetNorthing(sartDTOs.dPositionY)
            //    .Build();

            _sarts.Add(sartSignal);
            _isSartActive = isSartActive;
        }

        public void OnSimulationInitialized(SimulationInformation? simulationInformation)
        {
            if (simulationInformation == null)
            {
                return;
            }

            SimulationInitialized?.Invoke(this, new SimulationEventArgs(simulationInformation));
        }

        public void OnSimulationRunning(SimulationInformation? simulationInformation)
        {
            if (simulationInformation == null)
            {
                return;
            }

            SimulationRunning?.Invoke(this, new SimulationEventArgs(simulationInformation));
        }

        public void OnSimulationPaused(SimulationInformation? simulationInformation)
        {
            if (simulationInformation == null)
            {
                return;
            }

            SimulationPaused?.Invoke(this, new SimulationEventArgs(simulationInformation));
        }

        public void OnSimulationTerminated(SimulationInformation? simulationInformation)
        {
            if (simulationInformation == null)
            {
                return;
            }

            SimulationTerminated?.Invoke(this, new SimulationEventArgs(simulationInformation));
        }
    }
}
