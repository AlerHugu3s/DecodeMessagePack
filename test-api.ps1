# MessagePack Decoder Service API 测试脚本
Write-Host "正在测试 MessagePack Decoder Service..." -ForegroundColor Green

# 测试健康检查端点
Write-Host "`n1. 测试健康检查端点..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method GET
    Write-Host "✅ 健康检查成功:" -ForegroundColor Green
    $healthResponse | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ 健康检查失败: $($_.Exception.Message)" -ForegroundColor Red
}

# 测试主页
Write-Host "`n2. 测试主页..." -ForegroundColor Yellow
try {
    $homeResponse = Invoke-WebRequest -Uri "http://localhost:5000/" -Method GET
    Write-Host "✅ 主页访问成功: $($homeResponse.Content)" -ForegroundColor Green
} catch {
    Write-Host "❌ 主页访问失败: $($_.Exception.Message)" -ForegroundColor Red
}

# 测试Swagger文档
Write-Host "`n3. 测试Swagger文档..." -ForegroundColor Yellow
try {
    $swaggerResponse = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method GET
    Write-Host "✅ Swagger文档访问成功" -ForegroundColor Green
} catch {
    Write-Host "❌ Swagger文档访问失败: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n测试完成！" -ForegroundColor Green
Write-Host "您可以在浏览器中访问 http://localhost:5000/swagger 来查看API文档" -ForegroundColor Cyan
