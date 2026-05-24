using System.IO;

namespace TankBattleServer;

public class Match
{
    private readonly Dictionary<int, ServerTank> tanks = new();

    private readonly List<ServerCannonBall> cannonBalls = new();

    private int nextCannonBallId = 1;

    private const float HitRadius = 2f;
    private const int CannonBallDamage = 30;



    // 탱크 초기 위치 설정
    public Match()
    {
        tanks[1] = new ServerTank { PlayerId = 1, x = -3f, z = 0f, angle = 90f };

        tanks[2] = new ServerTank { PlayerId = 2, x = 3f, z = 0f, angle = -90f };
    }

    // 연결된 세션의 playerId 기준으로 판단.
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

            // 발사 쿨타임 감소
            if (tank.FireCooldown > 0f)
            {
                tank.FireCooldown -= deltaTime;
            }

            // Space를 누르고 있는 동안 매 Tick 마다 발사되지 않게 막기.
            bool firePressedThisFrame = tank.LastInput.Fire && tank.PrevFire == false;

            if (firePressedThisFrame && tank.FireCooldown <= 0f)
            {
                CreateCannonBall(tank);
                tank.FireCooldown = 0.5f;
            }

            // 이번 프레임 Fire 상태 저장
            tank.PrevFire = tank.LastInput.Fire;
        }

        // 포탄 이동
        foreach (ServerCannonBall cannonBall in cannonBalls)
        {
            cannonBall.Update(deltaTime);
        }

        CheckCannonBallHits();

        // 수명이 끝난 포탄 제거
        cannonBalls.RemoveAll(cannonBall => cannonBall.IsDead);
    }
    private void CreateCannonBall(ServerTank tank)
    {
        // 탱크 몸체 회전 + 포탑 회전을 합친 실제 발사 방향
        float yaw = tank.angle + tank.turretTurn;

        float yawRad = yaw * MathF.PI / 180f;
        float pitchRad = tank.gunPitch * MathF.PI / 180f;

        // X축, Z축에 대한 이동 방향 보정.
        float horizontal = MathF.Cos(pitchRad);

        float dirX = MathF.Sin(yawRad) * horizontal;
        float dirY = MathF.Sin(pitchRad);
        float dirZ = MathF.Cos(yawRad) * horizontal;

        // 탱크 로컬 기준 포신 끝 좌표
        const float muzzleLocalX = -1.25f;
        const float muzzleLocalY = 4f;
        const float muzzleLocalZ = 2.5f;

        // 포신 끝 좌표도 탱크 몸체 + 포탑 회전에 맞춰 회전
        float cos = MathF.Cos(yawRad);
        float sin = MathF.Sin(yawRad);

        //  탱크 월드 기준 포신 끝 좌표
        float muzzleWorldX = tank.x + muzzleLocalX * cos + muzzleLocalZ * sin;
        float muzzleWorldY = muzzleLocalY;
        float muzzleWorldZ = tank.z - muzzleLocalX * sin + muzzleLocalZ * cos;

        const float fireSpeed = 32f;

        // 새 대포 생성.
        ServerCannonBall cannonBall = new ServerCannonBall
        {
            CannonBallId = nextCannonBallId++, OwnerPlayerId = tank.PlayerId,
            
            X = muzzleWorldX,
            Y = muzzleWorldY,
            Z = muzzleWorldZ,

            VelX = dirX * fireSpeed,
            VelY = dirY * fireSpeed,
            VelZ = dirZ * fireSpeed,

            LifeTime = 0f
        };

        cannonBalls.Add(cannonBall);
    }

    // 탱크 상태 출력
    /*
    public void PrintStates()
    {
        foreach (ServerTank tank in tanks.Values)
        {
            Console.WriteLine($"P{tank.PlayerId} Pos=({tank.x:F2}, {tank.z:F2}), Yaw={tank.angle:F1}, Turret={tank.turretTurn:F1}, Gun={tank.gunPitch:F1}");
        }
    }*/

    public StatePacket CreateStatePacket() // 서버가 관리하는 탱크와 포탄 상태에 대한 상태 패킷 생성
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

        List<CannonBallState> cannonBallStates = new();

        foreach (ServerCannonBall cannonBall in cannonBalls)
        {
            CannonBallState state = new CannonBallState
            {
                CannonBallId = cannonBall.CannonBallId,
                OwnerPlayerId = cannonBall.OwnerPlayerId,
                X = cannonBall.X,
                Y = cannonBall.Y,
                Z = cannonBall.Z
            };

            cannonBallStates.Add(state);
        }

        return new StatePacket
        {
            Tanks = tankStates.ToArray(),
            CannonBalls = cannonBallStates.ToArray()
        };
    }
    private void CheckCannonBallHits()
    {
        foreach (ServerCannonBall cannonBall in cannonBalls)
        {
            foreach (ServerTank tank in tanks.Values)
            {
                if (tank.PlayerId == cannonBall.OwnerPlayerId)
                    continue;

                float dx = tank.x - cannonBall.X;
                float dz = tank.z - cannonBall.Z;
                float distanceSq = dx * dx + dz * dz;

                if (distanceSq <= HitRadius * HitRadius)
                {
                    tank.TakeDamage(CannonBallDamage);
                    cannonBall.LifeTime = 100f; // 죽은 포탄으로 만들기 위해 충분히 긴 시간으로 처리
                    break;
                }
            }
        }
    }
}