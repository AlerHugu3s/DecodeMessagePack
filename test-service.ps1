# MessagePack Decoder Service 测试脚本
Write-Host "正在启动 MessagePack Decoder Service..." -ForegroundColor Green

# 启动服务
Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory "F:\DecodeMessagePack" -WindowStyle Normal

# 等待服务启动
Start-Sleep -Seconds 5

Write-Host "服务已启动，可以通过以下URL访问：" -ForegroundColor Yellow
Write-Host "主页: http://localhost:5000" -ForegroundColor Cyan
Write-Host "健康检查: http://localhost:5000/health" -ForegroundColor Cyan
Write-Host "Swagger文档: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "API端点: http://localhost:5000/api/messagepack" -ForegroundColor Cyan

Write-Host "`n按任意键停止服务..." -ForegroundColor Red
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
