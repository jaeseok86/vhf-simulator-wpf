using GeoTimeZone;
using System;

namespace VHF_Client.Services.TimeZone
{
    public class TimeZoneLookupWrapper : ITimeZoneLookupWrapper
    {
        public DateTime GetLocalTime(double latitude, double longitude)
        {
            string timeZoneId = GetTimeZoneId(latitude, longitude);
            TimeZoneInfo timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
        }

        public string GetTimeZoneId(double latitude, double longitude)
        {
            return TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        }
    }
}

