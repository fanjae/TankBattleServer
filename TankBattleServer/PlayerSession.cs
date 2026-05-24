using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TankBattleServer.Packets;

namespace TankBattleServer;

public class PlayerSession
{
    public int PlayerId { get; }

    private const int MaxPacketSize = 4096;

    private readonly TcpClient client;
    private readonly NetworkStream stream;

    public PlayerSession(int playerId, TcpClient client)
    {
        PlayerId = playerId;
        this.client = client;

        // 데이터 송수신 스트림
        stream = client.GetStream();
    }

    public async Task SendJsonAsync<T>(T packet)
    {
        string json = JsonSerializer.Serialize(packet);


        byte[] body = Encoding.UTF8.GetBytes(json);

        // 본문 길이를 4바이트 정수로 변환
        byte[] lengthPrefix = BitConverter.GetBytes(body.Length);

        // 길이 4바이트 전송
        await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

        // JSON 본문 전송
        await stream.WriteAsync(body, 0, body.Length);

        Console.WriteLine($"Send to Player {PlayerId}: {json}");
    }
    public async Task ReceiveLoopAsync(Action<int, InputPacket> onInputReceived)
    {
        try
        {
            while (true)
            {
                // 패킷 수신
                string json = await ReceiveJsonAsync();

                // InputPacket 객체로 변환
                InputPacket? packet = JsonSerializer.Deserialize<InputPacket>(json);
                if (packet == null) continue;

                // 입력 수신에 대한 정보 전달
                onInputReceived(PlayerId, packet);
            }
        }
        catch (Exception e)
        {
            // 연결 종료 등에 대한 예외 처리
            Console.WriteLine($"Player {PlayerId} disconnected. Reason: {e.Message}");
        }
    }

    private async Task<string> ReceiveJsonAsync()
    {
        // 길이 정보 읽어서 int로 변환
        byte[] lengthBuffer = await ReadExactAsync(4);
        int bodyLength = BitConverter.ToInt32(lengthBuffer, 0);

        if (bodyLength <= 0 || bodyLength > MaxPacketSize)
        {
            throw new Exception($"Invalid packet size: {bodyLength}");
        }

        // 본문 정보 읽어서 JSON 문자열로 변환
        byte[] bodyBuffer = await ReadExactAsync(bodyLength);
        return Encoding.UTF8.GetString(bodyBuffer);
    }

    private async Task<byte[]> ReadExactAsync(int size)
    {
        // 지정한 크기 만큼의 데이터 담을 버퍼
        byte[] buffer = new byte[size];
        int offset = 0;

        // size 만큼 읽도록 반복
        while (offset < size)
        {
            int read = await stream.ReadAsync(buffer, offset, size - offset);

            if (read == 0)
            {
                throw new Exception("Client disconnected.");
            }

            // 읽어온 만큼 다음 기록 위치로 이동
            offset += read;
        }

        return buffer;
    }
    public void Close()
    {
        stream.Close();
        client.Close();
    }
}