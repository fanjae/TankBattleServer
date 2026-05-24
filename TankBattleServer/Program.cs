namespace TankBattleServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 7777 포트로 서버 생성
            GameServer server = new GameServer(port: 7777);

            // 서버 실행
            await server.StartAsync();
        }
    }
}
