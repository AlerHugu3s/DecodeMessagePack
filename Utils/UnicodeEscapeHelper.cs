using System.Text;
using System.Text.RegularExpressions;

namespace DecodeMessagePack.Utils;

public static class UnicodeEscapeHelper
{
    /// <summary>
    /// 转义JSON字符串中的Unicode控制字符
    /// </summary>
    /// <param name="jsonString">原始JSON字符串</param>
    /// <returns>转义后的JSON字符串</returns>
    public static string EscapeUnicodeControlCharacters(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return jsonString;

        // 匹配所有Unicode控制字符（除了已处理的 \r \n \t \f \b）
        var pattern = @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]";
        
        return Regex.Replace(jsonString, pattern, match =>
        {
            var charCode = (int)match.Value[0];
            return $"\\u{charCode:X4}";
        });
    }

    /// <summary>
    /// 安全地转换MessagePack为JSON，处理Unicode控制字符
    /// </summary>
    /// <param name="messagePackData">MessagePack字节数据</param>
    /// <returns>安全的JSON字符串</returns>
    public static string ConvertMessagePackToSafeJson(byte[] messagePackData)
    {
        try
        {
            // 首先尝试使用MessagePackSerializer.ConvertToJson
            var jsonString = MessagePack.MessagePackSerializer.ConvertToJson(messagePackData);
            
            // 转义Unicode控制字符
            return EscapeUnicodeControlCharacters(jsonString);
        }
        catch (Exception)
        {
            // 如果ConvertToJson失败，使用Deserialize + JsonSerializer的方式
            try
            {
                var jsonObject = MessagePack.MessagePackSerializer.Deserialize<object>(messagePackData);
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                return System.Text.Json.JsonSerializer.Serialize(jsonObject, jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"无法解析MessagePack数据: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 验证JSON字符串是否包含有效的Unicode字符
    /// </summary>
    /// <param name="jsonString">JSON字符串</param>
    /// <returns>是否有效</returns>
    public static bool IsValidJsonWithUnicode(string jsonString)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}
