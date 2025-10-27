using DecodeMessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加Worker服务（可选，用于后台任务）
builder.Services.AddHostedService<Worker>();

// 添加TCP MessagePack服务
builder.Services.AddHostedService<TcpMessagePackService>();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

// 添加健康检查端点
app.MapGet("/", () => "MessagePack Decoder Service is running!");
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.Now });

app.Run();