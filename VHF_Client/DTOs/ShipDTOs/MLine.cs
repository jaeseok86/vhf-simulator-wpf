namespace STRRadar.DTOs.ShipDTOs
{
    public struct MLine
    {
        public int m_bDisplay;
        public byte m_byTargetType;
        public ushort m_wChockId;
        public ushort m_wTargetId;
        public ushort m_wBollardId;
        public ushort m_wCommand;
        public double m_Tension;
        public ST_POINT_3D m_BollardPos;
    }
}
