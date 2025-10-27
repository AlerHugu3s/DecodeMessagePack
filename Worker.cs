namespace DecodeMessagePack;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Info] [{time}] MessagePack Decoder Service 已启动", DateTime.Now);
        
        // 由于这是一个Web API服务，Worker主要用于后台任务
        // 如果需要定期执行某些任务，可以在这里添加
        while (!stoppingToken.IsCancellationRequested)
        {
            // 可以在这里添加定期任务，比如清理临时文件等
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
        
        _logger.LogInformation("[Info] [{time}] MessagePack Decoder Service 正在关闭", DateTime.Now);
    }
}