# Test decode-from-bytes endpoint only
Write-Host "Testing decode-from-bytes endpoint..." -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Test health check first
Write-Host "`n1. Testing health endpoint..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET
    Write-Host "Health check successful:" -ForegroundColor Green
    $healthResponse | ConvertTo-Json
} catch {
    Write-Host "Health check failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Service might not be running. Please start the service first." -ForegroundColor Red
    exit 1
}

# Test decode-from-bytes endpoint
Write-Host "`n2. Testing decode-from-bytes endpoint..." -ForegroundColor Yellow
try {
    # Create test data - simple string converted to bytes
    $testString = "test messagepack data"
    $testData = [System.Text.Encoding]::UTF8.GetBytes($testString)
    
    $uri = "$baseUrl/api/messagepack/decode-from-bytes"
    Write-Host "Request URL: $uri" -ForegroundColor Cyan
    Write-Host "Test data size: $($testData.Length) bytes" -ForegroundColor Cyan
    Write-Host "Test data content: $testString" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $uri -Method POST -Body $testData -ContentType "application/octet-stream"
    Write-Host "decode-from-bytes test successful:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "decode-from-bytes test failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "HTTP Status Code: $statusCode" -ForegroundColor Red
        
        # Try to get more details about the error
        try {
            $errorResponse = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorResponse)
            $errorBody = $reader.ReadToEnd()
            Write-Host "Error details: $errorBody" -ForegroundColor Red
        } catch {
            Write-Host "Could not read error details" -ForegroundColor Red
        }
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green
