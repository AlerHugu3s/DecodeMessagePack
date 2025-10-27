# Simple MessagePack API Test
Write-Host "Testing MessagePack Decoder API..." -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Test health check
Write-Host "`n1. Testing health endpoint..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "Health check successful:" -ForegroundColor Green
    $healthResponse | ConvertTo-Json
} catch {
    Write-Host "Health check failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test decode-from-bytes endpoint
Write-Host "`n2. Testing decode-from-bytes endpoint..." -ForegroundColor Yellow
try {
    $testData = [System.Text.Encoding]::UTF8.GetBytes("test messagepack data")
    $uri = "$baseUrl/api/messagepack/decode-from-bytes"
    Write-Host "Request URL: $uri" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $uri -Method POST -Body $testData -ContentType "application/octet-stream"
    Write-Host "decode-from-bytes test successful:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "decode-from-bytes test failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "HTTP Status Code: $statusCode" -ForegroundColor Red
    }
}

# Test decode-from-string endpoint
Write-Host "`n3. Testing decode-from-string endpoint..." -ForegroundColor Yellow
try {
    $testString = "test messagepack string data"
    $uri = "$baseUrl/api/messagepack/decode-from-string"
    Write-Host "Request URL: $uri" -ForegroundColor Cyan
    Write-Host "Test string: $testString" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $uri -Method POST -Body $testString -ContentType "application/json"
    Write-Host "decode-from-string test successful:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "decode-from-string test failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "HTTP Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green
Write-Host "If there are still issues, please check:" -ForegroundColor Yellow
Write-Host "1. Service is running on correct port (http://localhost:5000)" -ForegroundColor White
Write-Host "2. Request data format is correct" -ForegroundColor White
Write-Host "3. MessagePack data is valid" -ForegroundColor White
