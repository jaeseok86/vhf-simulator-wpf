using System.Runtime.InteropServices;

namespace STRRadar.DTOs.ShipDTOs
{
    public struct ShipGun
    {
        public double dGunDeg;
        [MarshalAs(UnmanagedType.I1)]
        public bool bGunShot;
    }
}
