using System.Net;
using System.Net.Sockets;
using TankBattleServer.Packets;

namespace TankBattleServer;

public class GameServer
{
    private readonly int port;
    private readonly List<PlayerSession> players = new();
    private readonly Match match = new();

    public GameServer(int port)
    {
        this.port = port;
    }

    public async Task StartAsync()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine($"Server started. Port: {port}");

        while (players.Count < 2)
        {
            // 클라이언트 접속 대기
            TcpClient client = await listener.AcceptTcpClientAsync();

            // 플레이어 번호 할당 및 세션 생성
            int playerId = players.Count + 1;
            PlayerSession session = new PlayerSession(playerId, client);


            players.Add(session);
            Console.WriteLine($"Player {playerId} connected.");

            // 접속 완료에 대한 패킷 생성
            WelcomePacket welcomePacket = new WelcomePacket
            {
                PlayerId = playerId,
                Message = $"Welcome Player {playerId}"
            };

            // 전달
            await session.SendJsonAsync(welcomePacket);

            // 별도 Task 형태로 입력 수신 루프 시작
            _ = session.ReceiveLoopAsync(match.SetInput);
        }

        Console.WriteLine("Match started.");

        await GameLoopAsync();
    }

    private async Task GameLoopAsync()
    {
        // 서버에서 사용하는 Tick과 대기시간 설정
        const int tickRate = 30;
        const float deltaTime = 1f / tickRate;
        const int delayMs = 1000 / tickRate;

        while (true)
        {
            // 게임 상태 업데이트
            match.Update(deltaTime);

            // 현재 상태 설정
            StatePacket statePacket = match.CreateStatePacket();

            List<PlayerSession> disconnectedPlayers = new();

            // 상태 동기화를 위해 모든 플레이어게 상태 패킷 전송
            foreach (PlayerSession player in players)
            {
                try
                {
                    await player.SendJsonAsync(statePacket);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to send to Player {player.PlayerId}. Reason: {e.Message}");
                    disconnectedPlayers.Add(player); 
                }
            }

            foreach (PlayerSession player in disconnectedPlayers) // 연결 끊긴 플레이어 제거 처리
            {
                player.Close(); 
                players.Remove(player);
            }

            // 대기
            await Task.Delay(delayMs);
        }
    }
}