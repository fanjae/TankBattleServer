namespace TankBattleServer;

public class ServerCannonBall
{
    public int CannonBallId;
    public int OwnerPlayerId; // 포탄 발사 플레이어 ID

    public float X;
    public float Y;
    public float Z;

    public float VelX;
    public float VelY;
    public float VelZ;

    public float LifeTime;

    private const float Gravity = -9.81f; // 중력 가속도
    private const float MaxLifeTime = 3f; // 포탄 최대 생존시간

    // 일정 시간이 지났거나 지면에 닿으면 제거 대상으로 간주
    public bool IsExpired { get; private set; }
    public bool IsDead => IsExpired || LifeTime >= MaxLifeTime || Y <= 0f;

    public void Update(float deltaTime)
    {
        // 중력을 속도에 반영한 뒤, 현재 속도로 위치 갱신
        VelY += Gravity * deltaTime;

        X += VelX * deltaTime;
        Y += VelY * deltaTime;
        Z += VelZ * deltaTime;

        LifeTime += deltaTime;
    }
    public void MarkDead()
    {
        IsExpired = true;
    }
}