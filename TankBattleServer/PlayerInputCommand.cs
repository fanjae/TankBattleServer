using TankBattleServer.Packets;

namespace TankBattleServer;
public readonly struct PlayerInputCommand
{
    public int PlayerId { get; }
    public InputPacket Input { get; }

    public PlayerInputCommand(int playerId, InputPacket input)
    {
        PlayerId = playerId;
        Input = input;
    }
}