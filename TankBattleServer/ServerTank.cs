using TankBattleServer.Packets;

namespace TankBattleServer;

public class ServerTank
{
    public int PlayerId;

    public float x;
    public float z;
    public float angle;

    public float turretTurn;
    public float gunPitch;

    public int Hp = 100;

    // 클라이언트에게 받은 입력값
    public InputPacket LastInput = new();

    private const float TankSpeed = 5f;
    private const float ReverseSpeed = 3f;
    private const float TurnSpeed = 120f;
    private const float TurretTurnSpeed = 120f;
    private const float GunPitchSpeed = 20f;

    public bool PrevFire;
    public float FireCooldown;

    public void Update(float deltaTime)
    {
        UpdateBody(deltaTime);
        UpdateTurret(deltaTime);
        UpdateGun(deltaTime);
    }

    private void UpdateBody(float deltaTime) // 탱크 몸체 이동 및 회전 
    {
        float speed = LastInput.Move >= 0f ? TankSpeed : ReverseSpeed; // 전진 속도와 후진 속도 처리

        angle += LastInput.Turn * TurnSpeed * deltaTime;

        // 도 단위에서 라디안 단위로 변환
        float rad = angle * MathF.PI / 180f;

        // X축 이동 비율, Z축 이동 비율을 각각 구하기 위해 Sin, Cos으로 구함.
        x += MathF.Sin(rad) * LastInput.Move * speed * deltaTime;
        z += MathF.Cos(rad) * LastInput.Move * speed * deltaTime;
    }

    private void UpdateTurret(float deltaTime) // 터렛 좌우 회전
    {
        turretTurn += LastInput.Turret * TurretTurnSpeed * deltaTime;
    }

    private void UpdateGun(float deltaTime) // 포신 상하 회전
    {
        gunPitch += LastInput.Gun * GunPitchSpeed * deltaTime;
        gunPitch = Math.Clamp(gunPitch, -5f, 30f);
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0) Hp = 0;
    }
}
