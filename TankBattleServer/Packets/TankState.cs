namespace TankBattleServer.Packets;
public class TankState // 패킷 DTO
{
    public int PlayerId { get; set; }

    public float X { get; set; }
    public float Z { get; set; }
    public float Angle { get; set; }

    public float TurretTurn { get; set; }
    public float GunPitch { get; set; }

    public int Hp { get; set; }
}