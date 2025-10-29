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
    /// 将\uXXXX格式的转义序列转换回Unicode控制字符
    /// </summary>
    /// <param name="jsonString">包含\uXXXX转义序列的JSON字符串</param>
    /// <returns>转换后的JSON字符串</returns>
    public static string UnescapeUnicodeControlCharacters(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return jsonString;

        // 匹配\uXXXX格式的转义序列
        var pattern = @"\\u([0-9A-Fa-f]{4})";
        
        return Regex.Replace(jsonString, pattern, match =>
        {
            try
            {
                // 解析十六进制值
                var hexValue = match.Groups[1].Value;
                var charCode = Convert.ToInt32(hexValue, 16);
                
                // 转换为对应的Unicode字符
                return ((char)charCode).ToString();
            }
            catch
            {
                // 如果转换失败，返回原始匹配
                return match.Value;
            }
        });
    }
    
    /// <summary>
    /// 深度清理对象中的Unicode控制字符，特别处理字典的key
    /// </summary>
    /// <param name="obj">要清理的对象</param>
    /// <returns>清理后的对象</returns>
    public static object? CleanUnicodeControlCharacters(object? obj)
    {
        if (obj == null) return null;
        
        if (obj is string str)
        {
            // 清理字符串中的控制字符
            // return CleanStringControlCharacters(str);
        }
        else if (obj is System.Collections.IDictionary dict)
        {
            // 处理字典，特别清理key
            var cleanDict = new Dictionary<string, object?>();
            foreach (System.Collections.DictionaryEntry entry in dict)
            {
                
                if (entry.Key is Array keyArray)
                {
                    for (int i = 0; i < keyArray.Length; i++)
                    {
                        var key = keyArray.GetValue(i);
                        if (key is string strKey)
                        {
                            var cleanKey = CleanStringControlCharacters(strKey);
                            cleanDict[cleanKey] = entry.Value;
                            break;
                        }
                    }
                }
                else
                {
                    var cleanKey = CleanStringControlCharacters(entry.Key?.ToString() ?? "");
                    cleanDict[cleanKey] = entry.Value;
                }
            }
            return cleanDict;
        }
        else if (obj is System.Collections.IList list)
        {
            // 处理列表
            var cleanList = new List<object?>();
            foreach (var item in list)
            {
                cleanList.Add(CleanUnicodeControlCharacters(item));
            }
            return cleanList;
        }
        else if (obj is Array array)
        {
            // 处理数组
            var cleanArray = new object?[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                cleanArray[i] = CleanUnicodeControlCharacters(array.GetValue(i));
            }
            return cleanArray;
        }
        {
            // 其他类型直接返回
            return obj;
        }
    }

    /// <summary>
    /// 清理字符串中的Unicode控制字符，将控制字符替换为可见表示
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <returns>清理后的字符串</returns>
    public static string CleanStringControlCharacters(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        
        var result = new StringBuilder();
        foreach (char c in str)
        {
            if (char.IsControl(c) && c != '\r' && c != '\n' && c != '\t')
            {
                // 将控制字符替换为可见的十六进制表示
                result.Append($"[\\u{(int)c:X4}]");
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// 安全地转换MessagePack为JSON，处理Unicode控制字符
    /// </summary>
    /// <param name="messagePackData">MessagePack字节数据</param>
    /// <returns>安全的JSON字符串</returns>
    public static string ConvertMessagePackToSafeJson(byte[] messagePackData)
    {
        var jsonObject = MessagePack.MessagePackSerializer.Deserialize<object>(messagePackData);
                
        // 深度清理对象中的Unicode控制字符
        var cleanObject = CleanUnicodeControlCharacters(jsonObject);
                
        // 使用Newtonsoft.Json处理清理后的对象
        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cleanObject, Newtonsoft.Json.Formatting.Indented);
        
        return jsonString;
    }
}
