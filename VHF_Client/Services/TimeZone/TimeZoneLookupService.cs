using GeoTimeZone;
using System;

namespace VHF_Client.Services.TimeZone
{
    public class TimeZoneLookupService
    {
        private readonly ITimeZoneLookupWrapper _timeZoneLookup;

        public TimeZoneLookupService(ITimeZoneLookupWrapper timeZoneLookup)
        {
            _timeZoneLookup = timeZoneLookup;
        }

        public DateTime GetLocalTime(double latitude, double longitude)
        {
            return _timeZoneLookup.GetLocalTime(latitude, longitude);
        }

        public DateTime GetLocalTime(double latitude, double longitude, long unixTime)
        {
            // IOS가 무조건 한국 시간대 기준으로 보내기 때문에 추후에 업데이트가 되지 않는다면 이걸 사용할 것.
            // TimeZoneInfo kstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");

            // 위경도를 통해 시간대 찾기
            var timeZoneId = GetTimeZoneId(latitude, longitude);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            // Unix epoch 기준의 시간 계산
            var simulationDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            // 해당 지역의 표준시로 변환
            var localTime = TimeZoneInfo.ConvertTime(simulationDateTimeUtc, timeZone);

            return localTime.DateTime;
        }

        private string GetTimeZoneId(double latitude, double longitude)
        {
            return _timeZoneLookup.GetTimeZoneId(latitude, longitude);
        }

        public static DateTime ConvertLocalTimeToUtc(double latitude, double longitude, DateTime localTime)
        {
            var timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);

            return utcTime;
        }
    }
}
