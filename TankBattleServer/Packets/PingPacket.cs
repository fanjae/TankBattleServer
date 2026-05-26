namespace TankBattleServer.Packets;

public class PingPacket
{
    public string Type { get; set; } = "Ping";
    public long ClientTimeMs { get; set; }
}