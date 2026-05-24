namespace TankBattleServer;

public class Match
{
    private readonly Dictionary<int, ServerTank> tanks = new();

    // 탱크 초기 위치 설정
    public Match()
    {
        tanks[1] = new ServerTank { PlayerId = 1, x = -3f, z = 0f, angle = 90f };

        tanks[2] = new ServerTank { PlayerId = 2, x = 3f, z = 0f, angle = -90f };
    }

    // 탱크 입력 확인
    public void SetInput(int playerId, InputPacket input)
    {
        if (tanks.TryGetValue(playerId, out ServerTank? tank) == false) return;

        tank.LastInput = input;
    }

    // Tick 단위로 탱크 상태 갱신
    public void Update(float deltaTime)
    {
        foreach (ServerTank tank in tanks.Values)
        {
            tank.Update(deltaTime);
        }
    }

    // 탱크 상태 출력
    public void PrintStates()
    {
        foreach (ServerTank tank in tanks.Values)
        {
            Console.WriteLine($"P{tank.PlayerId} Pos=({tank.x:F2}, {tank.z:F2}), Yaw={tank.angle:F1}, Turret={tank.turretTurn:F1}, Gun={tank.gunPitch:F1}");
        }
    }

    public StatePacket CreateStatePacket()
    {
        List<TankState> tankStates = new();

        foreach (ServerTank tank in tanks.Values)
        {
            TankState state = new TankState
            {
                PlayerId = tank.PlayerId,

                X = tank.x,
                Z = tank.z,
                Angle = tank.angle,

                TurretTurn = tank.turretTurn,
                GunPitch = tank.gunPitch,

                Hp = tank.Hp
            };

            tankStates.Add(state);
        }

        return new StatePacket { Tanks = tankStates.ToArray() };
    }
}