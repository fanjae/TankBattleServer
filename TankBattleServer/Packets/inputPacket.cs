public class InputPacket // 플레이어 입력 패킷
{
    public string Type { get; set; } = "Input";

    public float Move { get; set; }
    public float Turn { get; set; }
    public float Turret { get; set; }
    public float Gun { get; set; }

    public bool Fire { get; set; }
}