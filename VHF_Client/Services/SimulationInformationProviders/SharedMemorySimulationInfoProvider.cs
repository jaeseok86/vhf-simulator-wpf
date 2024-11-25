using STRRadar.DTOs.ShipDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VHF_Client.Common;
using VHF_Client.Services.SharedMemories;
using VHF_Client.Services.TimeZone;

namespace VHF_Client.Services.SimulationInformationProviders
{
    public class SharedMemorySimulationInfoProvider
    {
        private SharedMemoryService _sharedMemoryService;
        private TimeZoneLookupService _timeZoneLookupService;

        private string latitudeInfo = "";
        private string longitudeInfo = "";

        Mediator mediator = Mediator.instance;

        System.Timers.Timer simulatorTimer = null;

        public SharedMemorySimulationInfoProvider()
        {
            _sharedMemoryService = new SharedMemoryService(new SharedMemoryReaderWrapper());
            _timeZoneLookupService = new TimeZoneLookupService(new TimeZoneLookupWrapper());

            InitSimulatorTimer();
        }

        private void InitSimulatorTimer()
        {
            simulatorTimer = new System.Timers.Timer(100);
            simulatorTimer.Elapsed += SimulatorTimer_Elapsed;
            simulatorTimer.AutoReset = true;
            simulatorTimer.Start();
        }

        public void StopSimualtorTimer()
        {
            if(simulatorTimer != null)
            {
                simulatorTimer.Stop();
                simulatorTimer.Elapsed -= SimulatorTimer_Elapsed;
                simulatorTimer.AutoReset = false;
                simulatorTimer.Dispose();
            }
        }

        private void SimulatorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetSharedMemoryInfo();
        }

        public void GetSharedMemoryInfo()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string localtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mediator.NotifyLoacaltimeChanged(localtime);

                if (!_sharedMemoryService.HasData<ST_OShip>("STR_OWNSHIP_DATA"))
                {
                    return;
                }

                ST_OShip simulationInformation = _sharedMemoryService.ReadData<ST_OShip>("STR_BCAST_DATA");

                double latitude = simulationInformation.m_LatLon.dX;
                double longitude = simulationInformation.m_LatLon.dY;

                ConvertToDMS(latitude, longitude);

                //DateTime simulationTime = _timeZoneLookupService.GetLocalTime(
                //    simulationInformation.m_LatLon.dX,
                //    simulationInformation.m_LatLon.dY,
                //    (long)simulationInformation.m_SimulationTime); // 위 경도 및 해당 표준시 simulatrion time 사용

                mediator.NotifyLatitudeChanged(latitudeInfo);
                mediator.NotifyLongitudeChanged(longitudeInfo);
                //mediator.NotifyLoacaltimeChanged(simulationTime.ToString());
            });
        }

        // 위도와 경도를 변환하는 메서드
        public void ConvertToDMS(double latitude, double longitude)
        {
            // 위도 변환
            latitudeInfo = ConvertToDMS(latitude, true);

            // 경도 변환
            longitudeInfo = ConvertToDMS(longitude, false);

            //return $"{latitudeDMS}, {longitudeDMS}";
        }

        // DMS(도, 분, 초) 형식으로 변환하는 메서드
        private static string ConvertToDMS(double decimalDegree, bool isLatitude)
        {
            // 도(degree) 계산
            int degrees = (int)decimalDegree;
            // 분 계산
            double minutesDecimal = (decimalDegree - degrees) * 60;
            int minutes = (int)minutesDecimal;
            // 초 계산
            double seconds = (minutesDecimal - minutes) * 60;

            // 방향 설정
            string direction;
            if (isLatitude)
            {
                direction = degrees >= 0 ? "N" : "S";
            }
            else
            {
                direction = degrees >= 0 ? "E" : "W";
            }

            // 절대값 사용하여 방향에 맞게 값 처리
            degrees = Math.Abs(degrees);
            minutes = Math.Abs(minutes);
            seconds = Math.Abs(seconds);

            // 도, 분, 초를 형식에 맞춰 변환
            return $"{degrees:000}:{minutes:00}:{seconds:00.00}{direction}";
        }
    }
}
