# MessagePack Decoder Service

这是一个C# Web API服务，用于解析MessagePack数据文件并输出为JSON格式。

## 功能特性

- 支持通过文件上传解析MessagePack数据
- 支持通过字节数组解析MessagePack数据
- 输出格式化的JSON文本
- 提供健康检查端点
- 支持Swagger API文档（开发环境）

## API端点

### 1. 文件上传解析
- **URL**: `POST /api/messagepack/decode`
- **Content-Type**: `multipart/form-data`
- **参数**: `file` (MessagePack文件)
- **返回**: JSON格式的解析结果

### 2. 字节数组解析
- **URL**: `POST /api/messagepack/decode-from-bytes`
- **Content-Type**: `application/octet-stream`
- **参数**: MessagePack字节数组
- **返回**: JSON格式的解析结果

### 3. 健康检查
- **URL**: `GET /health`
- **返回**: 服务状态信息

## 运行服务

### 方法1：直接运行
```bash
# 在项目目录中运行
dotnet run
```

### 方法2：使用测试脚本（Windows PowerShell）
```powershell
# 运行测试脚本
.\test-service.ps1
```

### 方法3：构建后运行
```bash
# 构建项目
dotnet build

# 运行构建后的程序
dotnet bin/Debug/net9.0/DecodeMessagePack.dll
```

服务将在以下地址启动：
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`（如果配置了SSL）

## 使用示例

### 使用curl上传文件
```bash
curl -X POST "https://localhost:5001/api/messagepack/decode" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@your-messagepack-file.msgpack"
```

### 使用curl发送字节数据
```bash
curl -X POST "https://localhost:5001/api/messagepack/decode-from-bytes" \
  -H "accept: application/json" \
  -H "Content-Type: application/octet-stream" \
  --data-binary @your-messagepack-file.msgpack
```

## 返回格式

成功解析时返回：
```json
{
  "success": true,
  "fileName": "example.msgpack",
  "fileSize": 1024,
  "jsonData": "{\n  \"key\": \"value\",\n  \"number\": 123\n}"
}
```

解析失败时返回：
```json
{
  "success": false,
  "error": "解析MessagePack文件失败",
  "details": "具体错误信息"
}
```

## 开发环境

在开发环境中，可以通过 `https://localhost:5001/swagger` 访问Swagger UI来测试API。
