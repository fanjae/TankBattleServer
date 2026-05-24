public class StatePacket
{
    public string Type { get; set; } = "State";

    public TankState[] Tanks { get; set; } = Array.Empty<TankState>();

    public CannonBallState[] CannonBalls { get; set; } = Array.Empty<CannonBallState>();
}