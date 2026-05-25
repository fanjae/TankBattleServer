namespace TankBattleServer.Packets;
public class CannonBallState
{
    public int CannonBallId { get; set; }
    public int OwnerPlayerId { get; set; }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}