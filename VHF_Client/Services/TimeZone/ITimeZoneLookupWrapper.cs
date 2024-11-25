using System;

namespace VHF_Client.Services.TimeZone
{
    public interface ITimeZoneLookupWrapper
    {
        DateTime GetLocalTime(double latitude, double longitude);
        string GetTimeZoneId(double latitude, double longitude);
    }
}
