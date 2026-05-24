namespace TankBattleServer.Packets;
public class WelcomePacket
{
    public string Type { get; set; } = "Welcome";
    public int PlayerId { get; set; }
    public string Message { get; set; } = "";
}