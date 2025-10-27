# MessagePack Decoder Service

这是一个C# Web API服务，用于解析MessagePack数据文件并输出为JSON格式。

## 🚀 快速开始

1. **克隆或下载项目**
2. **运行服务**：
   ```bash
   dotnet run
   ```
3. **访问API文档**：http://localhost:5000/swagger
4. **测试健康检查**：http://localhost:5000/health

## 功能特性

- ✅ 支持通过文件上传解析MessagePack数据
- ✅ 支持通过字节数组解析MessagePack数据
- ✅ 支持通过字符串解析MessagePack数据
- ✅ 输出格式化的JSON文本
- ✅ 提供健康检查端点
- ✅ 支持Swagger API文档（开发环境）
- ✅ 完整的错误处理和日志记录
- ✅ 支持多种输入格式
- ✅ 支持LZ4压缩的MessagePack数据
- ✅ 支持多种MessagePack解析器

## API端点

### 1. 文件上传解析
- **URL**: `POST /api/messagepack/decode`
- **Content-Type**: `multipart/form-data`
- **参数**: `file` (MessagePack文件) - 必需
- **返回**: JSON格式的解析结果

### 2. 字节数组解析
- **URL**: `POST /api/messagepack/decode-from-bytes`
- **Content-Type**: `application/octet-stream`
- **参数**: MessagePack字节数组 (请求体)
- **返回**: JSON格式的解析结果

### 3. 字符串解析
- **URL**: `POST /api/messagepack/decode-from-string`
- **Content-Type**: `application/json`
- **参数**: MessagePack字符串 (请求体)
- **返回**: JSON格式的解析结果

### 4. 健康检查
- **URL**: `GET /health`
- **返回**: 服务状态信息

## 运行服务

### 方法1：直接运行（推荐）
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

### 方法4：指定端口运行
```bash
# 只使用HTTP端口
dotnet run --urls "http://localhost:5000"
```

## 服务地址

服务启动后将在以下地址运行：
- **HTTP**: `http://localhost:5000`
- **Swagger文档**: `http://localhost:5000/swagger`
- **健康检查**: `http://localhost:5000/health`

## 常见问题解决

### HTTPS证书问题
如果遇到HTTPS证书错误，可以：

1. **只使用HTTP**（推荐用于开发）：
   ```bash
   dotnet run --urls "http://localhost:5000"
   ```

2. **生成开发证书**：
   ```bash
   # 生成证书
   dotnet dev-certs https
   
   # 信任证书（Windows/macOS）
   dotnet dev-certs https --trust
   ```

3. **修改launchSettings.json**：
   ```json
   "applicationUrl": "http://localhost:5000"
   ```

## 支持的格式

### MessagePack解析器
服务支持多种MessagePack解析器：
- **StandardResolver** - 标准解析器
- **UnityResolver** - Unity游戏引擎解析器
- **BuiltinResolver** - 内置解析器
- **AttributeFormatterResolver** - 属性格式化解析器
- **PrimitiveObjectResolver** - 原始对象解析器

### 压缩支持
- **LZ4压缩** - 支持LZ4Block压缩的MessagePack数据
- **无压缩** - 支持标准MessagePack数据

## 使用示例

### 使用curl上传文件
```bash
curl -X POST "http://localhost:5000/api/messagepack/decode" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@your-messagepack-file.msgpack"
```

### 使用curl发送字节数据
```bash
curl -X POST "http://localhost:5000/api/messagepack/decode-from-bytes" \
  -H "accept: application/json" \
  -H "Content-Type: application/octet-stream" \
  --data-binary @your-messagepack-file.msgpack
```

### 使用curl发送字符串数据
```bash
curl -X POST "http://localhost:5000/api/messagepack/decode-from-string" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d "your-messagepack-string-data"
```

### 使用PowerShell测试
```powershell
# 测试健康检查
Invoke-RestMethod -Uri "http://localhost:5000/health" -Method GET

# 测试文件上传
$file = Get-Item "your-file.msgpack"
$form = @{file = $file}
Invoke-RestMethod -Uri "http://localhost:5000/api/messagepack/decode" -Method POST -Form $form

# 测试字节数组上传
$bytes = [System.IO.File]::ReadAllBytes("your-file.msgpack")
Invoke-RestMethod -Uri "http://localhost:5000/api/messagepack/decode-from-bytes" -Method POST -Body $bytes -ContentType "application/octet-stream"

# 测试字符串上传
$stringData = "your-messagepack-string-data"
Invoke-RestMethod -Uri "http://localhost:5000/api/messagepack/decode-from-string" -Method POST -Body $stringData -ContentType "application/json"
```

### 使用浏览器测试
1. 打开浏览器访问 `http://localhost:5000/swagger`
2. 在Swagger UI中测试API端点
3. 上传MessagePack文件进行解析

## 返回格式

### 成功解析时返回：
```json
{
  "success": true,
  "fileName": "example.msgpack",
  "fileSize": 1024,
  "jsonData": "{\n  \"key\": \"value\",\n  \"number\": 123\n}"
}
```

### 字节数组解析成功时返回：
```json
{
  "success": true,
  "dataSize": 1024,
  "jsonData": "{\n  \"key\": \"value\",\n  \"number\": 123\n}"
}
```

### 字符串解析成功时返回：
```json
{
  "success": true,
  "stringLength": 50,
  "dataSize": 50,
  "jsonData": "{\n  \"key\": \"value\",\n  \"number\": 123\n}"
}
```

### 解析失败时返回：
```json
{
  "success": false,
  "error": "解析MessagePack文件失败",
  "details": "具体错误信息"
}
```

### 常见错误类型：
- **缺少文件**: "请选择一个MessagePack文件"
- **数据为空**: "MessagePack数据不能为空"
- **字符串为空**: "MessagePack字符串不能为空"
- **解析失败**: "MessagePack数据格式错误"
- **序列化错误**: "MessagePack序列化失败"

## 项目结构

```
DecodeMessagePack/
├── Controllers/
│   └── MessagePackController.cs    # API控制器
├── Properties/
│   └── launchSettings.json         # 启动配置
├── Program.cs                      # 应用程序入口
├── Worker.cs                       # 后台服务
├── DecodeMessagePack.csproj        # 项目文件
├── README.md                       # 说明文档
├── test-service.ps1               # 服务启动脚本
├── test-simple.ps1                # 简单测试脚本
└── test-api.ps1                   # API测试脚本
```

## 技术栈

- **.NET 9.0** - 运行时框架
- **ASP.NET Core** - Web API框架
- **MessagePack** - MessagePack序列化库
- **LZ4压缩** - MessagePack压缩支持
- **多种解析器** - 支持多种MessagePack解析器
- **Swagger/OpenAPI** - API文档生成
- **System.Text.Json** - JSON序列化

## 开发环境

在开发环境中，可以通过 `http://localhost:5000/swagger` 访问Swagger UI来测试API。

## 日志格式

服务使用统一的日志格式：`[Level] [Time] Content`
- **Level**: Error, Warning, Info
- **Time**: 本地时区时间
- **Content**: 日志消息内容

## 许可证

本项目仅供学习和开发使用。
