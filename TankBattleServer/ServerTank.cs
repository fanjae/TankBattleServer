using TankBattleServer.Packets;

namespace TankBattleServer;

public class ServerTank
{
    public int PlayerId { get; }

    public float X { get; private set; }
    public float Z { get; private set; }
    public float Angle { get; private set; }

    public float TurretTurn { get; private set; }
    public float GunPitch { get; private set; }

    public int Hp { get; private set; } = 100;

    public bool PrevFire { get; set; }
    public float FireCooldown { get; set; }

    // 클라이언트에게 받은 입력값
    public InputPacket LastInput { get; private set; } = new();

    private const float TankSpeed = 5f;
    private const float ReverseSpeed = 3f;
    private const float TurnSpeed = 120f;
    private const float TurretTurnSpeed = 120f;
    private const float GunPitchSpeed = 20f;

    public ServerTank(int playerId, float x, float z, float angle)
    {
        PlayerId = playerId;
        this.X = x;
        this.Z = z;
        this.Angle = angle;
    }

    public void SetInput(InputPacket input)
    {
        LastInput = input;
    }

    public void Update(float deltaTime)
    {
        UpdateBody(deltaTime);
        UpdateTurret(deltaTime);
        UpdateGun(deltaTime);
    }

    private void UpdateBody(float deltaTime) // 탱크 몸체 이동 및 회전 
    {
        float move = Math.Clamp(LastInput.Move, -1f, 1f);
        float turn = Math.Clamp(LastInput.Turn, -1f, 1f);

        float speed = LastInput.Move >= 0f ? TankSpeed : ReverseSpeed; // 전진 속도와 후진 속도 처리

        Angle += LastInput.Turn * TurnSpeed * deltaTime;

        // 도 단위에서 라디안 단위로 변환
        float rad = Angle * MathF.PI / 180f;

        // X축 이동 비율, Z축 이동 비율을 각각 구하기 위해 Sin, Cos으로 구함.
        X += MathF.Sin(rad) * LastInput.Move * speed * deltaTime;
        Z += MathF.Cos(rad) * LastInput.Move * speed * deltaTime;
    }

    private void UpdateTurret(float deltaTime) // 터렛 좌우 회전
    {
        float turret = Math.Clamp(LastInput.Turret, -1f, 1f);

        TurretTurn += LastInput.Turret * TurretTurnSpeed * deltaTime;
    }

    private void UpdateGun(float deltaTime) // 포신 상하 회전
    {
        float gun = Math.Clamp(LastInput.Gun, -1f, 1f);

        GunPitch += LastInput.Gun * GunPitchSpeed * deltaTime;
        GunPitch = Math.Clamp(GunPitch, -5f, 30f);
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0) Hp = 0;
    }
}
