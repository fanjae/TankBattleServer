namespace TankBattleServer.Packets;

public class PongPacket
{
    public string Type { get; set; } = "Pong";
    public long ClientTimeMs { get; set; }
}