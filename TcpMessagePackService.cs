using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessagePack;
using DecodeMessagePack.Utils;

namespace DecodeMessagePack;

public class TcpMessagePackService(ILogger<TcpMessagePackService> logger) : BackgroundService
{
    private TcpListener? _listener;
    private const int Port = 8888;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();
        
        logger.LogInformation("[Info] [{time}] TCP MessagePack服务已启动，监听端口: {port}", DateTime.Now, Port);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(stoppingToken);
                _ = Task.Run(() => HandleClientAsync(tcpClient, stoppingToken), stoppingToken);
            }
            catch (ObjectDisposedException)
            {
                // 服务正在关闭
                break;
            }
            catch (Exception ex)
            {
                logger.LogError("[Error] [{time}] 接受TCP连接失败: {error}", DateTime.Now, ex.Message);
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using (client)
        await using (var stream = client.GetStream())
        {
            try
            {
                logger.LogInformation("[Info] [{time}] 新客户端连接: {endpoint}", DateTime.Now, client.Client.RemoteEndPoint);

                // 读取数据长度（前4字节）
                var lengthBytes = new byte[4];
                await stream.ReadExactlyAsync(lengthBytes, 0, 4, cancellationToken);
                var dataLength = BitConverter.ToInt32(lengthBytes, 0);

                // 读取MessagePack数据
                var messagePackData = new byte[dataLength];
                var totalBytesRead = 0;
                while (totalBytesRead < dataLength)
                {
                    var bytesRead = await stream.ReadAsync(messagePackData, totalBytesRead, dataLength - totalBytesRead, cancellationToken);
                    if (bytesRead == 0)
                        throw new Exception("连接意外关闭");
                    totalBytesRead += bytesRead;
                }

                logger.LogInformation("[Info] [{time}] 接收到MessagePack数据，大小: {size} bytes", DateTime.Now, dataLength);

                // 使用工具类安全地转换MessagePack为JSON
                var jsonString = UnicodeEscapeHelper.ConvertMessagePackToSafeJson(messagePackData);

                // 创建响应
                var response = new
                {
                    success = true,
                    dataSize = dataLength,
                    jsonData = jsonString
                };

                var responseJson = JsonSerializer.Serialize(response);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);
                var responseLengthBytes = BitConverter.GetBytes(responseBytes.Length);

                // 发送响应长度
                await stream.WriteAsync(responseLengthBytes, 0, 4, cancellationToken);
                // 发送响应数据
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);

                logger.LogInformation("[Info] [{time}] MessagePack数据解析成功", DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.LogError("[Error] [{time}] 处理客户端请求失败: {error}", DateTime.Now, ex.Message);

                try
                {
                    var errorResponse = new
                    {
                        success = false,
                        error = "解析MessagePack数据失败",
                        details = ex.Message
                    };

                    var errorJson = JsonSerializer.Serialize(errorResponse);
                    var errorBytes = Encoding.UTF8.GetBytes(errorJson);
                    var errorLengthBytes = BitConverter.GetBytes(errorBytes.Length);

                    await stream.WriteAsync(errorLengthBytes, 0, 4, cancellationToken);
                    await stream.WriteAsync(errorBytes, 0, errorBytes.Length, cancellationToken);
                }
                catch
                {
                    // 忽略发送错误响应的异常
                }
            }
        }
    }

    public override void Dispose()
    {
        _listener?.Stop();
        base.Dispose();
    }
}
