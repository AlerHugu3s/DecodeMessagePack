# MessagePack解码API测试脚本
Write-Host "测试MessagePack解码API..." -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# 测试健康检查
Write-Host "`n1. 测试健康检查..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "✅ 健康检查成功" -ForegroundColor Green
    $healthResponse | ConvertTo-Json
} catch {
    Write-Host "❌ 健康检查失败: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 测试decode-from-bytes端点
Write-Host "`n2. 测试decode-from-bytes端点..." -ForegroundColor Yellow
try {
    # 创建测试数据（这里使用简单的字节数组）
    $testData = [System.Text.Encoding]::UTF8.GetBytes("test messagepack data")
    
    $uri = "$baseUrl/api/messagepack/decode-from-bytes"
    Write-Host "请求URL: $uri" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $uri -Method POST -Body $testData -ContentType "application/octet-stream"
    Write-Host "✅ decode-from-bytes测试成功" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ decode-from-bytes测试失败: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "HTTP状态码: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`n测试完成！" -ForegroundColor Green
Write-Host "如果仍有问题，请检查：" -ForegroundColor Yellow
Write-Host "1. 服务是否运行在正确的端口 (http://localhost:5000)" -ForegroundColor White
Write-Host "2. 请求数据格式是否正确" -ForegroundColor White
Write-Host "3. MessagePack数据是否有效" -ForegroundColor White
