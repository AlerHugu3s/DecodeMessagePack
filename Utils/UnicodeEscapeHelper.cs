using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        // try
        // {
        //     // 使用MessagePackSerializer.ConvertToJson，这是最可靠的方法
        //     var jsonString = MessagePack.MessagePackSerializer.ConvertToJson(messagePackData);
        //     
        //     // 转义Unicode控制字符
        //     return EscapeUnicodeControlCharacters(jsonString);
        // }
        // catch (Exception ex)
        // {
        //     // 如果ConvertToJson失败，尝试使用Newtonsoft.Json作为备选方案
        //     try
        //     {
                var jsonObject = MessagePack.MessagePackSerializer.Deserialize<object>(messagePackData);
                
                // 使用Newtonsoft.Json处理复杂对象
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
                
                // 转义Unicode控制字符
                return EscapeUnicodeControlCharacters(jsonString);
            // }
            // catch (Exception innerEx)
            // {
            //     throw new InvalidOperationException($"无法解析MessagePack数据: {ex.Message}. 备选方案也失败: {innerEx.Message}", ex);
            // }
    }
}
