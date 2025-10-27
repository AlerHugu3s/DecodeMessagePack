using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MessagePack;
using System.Text.Json;
using DecodeMessagePack.Library;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace DecodeMessagePack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagePackController : ControllerBase
{
    private readonly ILogger<MessagePackController> _logger;

    public MessagePackController(ILogger<MessagePackController> logger)
    {
        _logger = logger;
    }
    
   

    [HttpPost("decode")]
    public async Task<IActionResult> DecodeMessagePack(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("请选择一个MessagePack文件");
            }
            
            var resolver = CompositeResolver.Create(
                new IFormatterResolver[]
                {
                    // use generated resolver first, and combine many other generated/custom resolvers
                    // GeneratedMessagePackResolver.Instance,
                    StandardResolver.Instance,
					
                    // StandardResolver.Instance,

                    MessagePack.Unity.UnityResolver.Instance,

                    // finally, use builtin/primitive resolver(don't use StandardResolver, it includes dynamic generation)
                    BuiltinResolver.Instance,
                    AttributeFormatterResolver.Instance,
                    PrimitiveObjectResolver.Instance,
                }
            );
            var options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver)
                .WithCompression(MessagePackCompression.Lz4Block);
            // options.WithCompression(MessagePackCompression.None);
            MessagePackSerializer.DefaultOptions = options;

            _logger.LogInformation("[Info] [{time}] 开始解析MessagePack文件: {fileName}", DateTime.Now, file.FileName);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var messagePackData = memoryStream.ToArray();

            // 解析MessagePack数据
            var jsonString = MessagePackSerializer.ConvertToJson(messagePackData);

            _logger.LogInformation("[Info] [{time}] MessagePack文件解析成功: {fileName}", DateTime.Now, file.FileName);

            return Ok(new
            {
                success = true,
                fileName = file.FileName,
                fileSize = file.Length,
                jsonData = jsonString
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("[Error] [{time}] 解析MessagePack文件失败: {error}", DateTime.Now, ex.Message);
            return StatusCode(500, new
            {
                success = false,
                error = "解析MessagePack文件失败",
                details = ex.Message
            });
        }
    }

    [HttpPost("decode-from-bytes")]
    public async Task<IActionResult> DecodeFromBytes()
    {
        try
        {
            // 从请求体读取原始字节数据
            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            var messagePackData = memoryStream.ToArray();
            
            var resolver = CompositeResolver.Create(
                new IFormatterResolver[]
                {
                    // use generated resolver first, and combine many other generated/custom resolvers
                    // GeneratedMessagePackResolver.Instance,
                    StandardResolver.Instance,
					
                    // StandardResolver.Instance,

                    MessagePack.Unity.UnityResolver.Instance,

                    // finally, use builtin/primitive resolver(don't use StandardResolver, it includes dynamic generation)
                    BuiltinResolver.Instance,
                    AttributeFormatterResolver.Instance,
                    PrimitiveObjectResolver.Instance,
                }
            );
            var options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver)
                .WithCompression(MessagePackCompression.Lz4Block);
            // options.WithCompression(MessagePackCompression.None);
            MessagePackSerializer.DefaultOptions = options;

            if (messagePackData == null || messagePackData.Length == 0)
            {
                return BadRequest("MessagePack数据不能为空");
            }

            _logger.LogInformation("[Info] [{time}] 开始解析MessagePack字节数据，大小: {size} bytes", DateTime.Now, messagePackData.Length);
            
            // 解析MessagePack数据
            var jsonString = MessagePackSerializer.ConvertToJson(messagePackData);
            
            _logger.LogInformation("[Info] [{time}] MessagePack字节数据解析成功", DateTime.Now);

            return Ok(new
            {
                success = true,
                dataSize = messagePackData.Length,
                jsonData = jsonString
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("[Error] [{time}] 解析MessagePack字节数据失败: {error}", DateTime.Now, ex.Message);
            return StatusCode(500, new
            {
                success = false,
                error = "解析MessagePack字节数据失败",
                details = ex.Message
            });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.Now,
            service = "MessagePack Decoder Service"
        });
    }
}
