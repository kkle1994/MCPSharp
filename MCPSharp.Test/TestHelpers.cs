using MCPSharp.Model.Content;
using System.Text.Json;

namespace MCPSharp.Test
{
    /// <summary>
    /// 测试辅助方法
    /// </summary>
    public static class TestHelpers
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        /// <summary>
        /// 从内容对象获取文本值
        /// </summary>
        public static string GetText(object content)
        {
            if (content is JsonElement jsonElement)
            {
                return jsonElement.GetProperty("text").GetString() ?? string.Empty;
            }
            
            var textProp = content.GetType().GetProperty("Text");
            return textProp?.GetValue(content)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 从内容对象获取类型值
        /// </summary>
        public static string GetType(object content)
        {
            if (content is JsonElement jsonElement)
            {
                return jsonElement.GetProperty("type").GetString() ?? string.Empty;
            }
            
            var typeProp = content.GetType().GetProperty("Type");
            return typeProp?.GetValue(content)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 从内容对象获取 MIME 类型
        /// </summary>
        public static string GetMimeType(object content)
        {
            if (content is JsonElement jsonElement)
            {
                return jsonElement.GetProperty("mimeType").GetString() ?? string.Empty;
            }
            
            var mimeTypeProp = content.GetType().GetProperty("MimeType");
            return mimeTypeProp?.GetValue(content)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 从内容对象获取数据
        /// </summary>
        public static string GetData(object content)
        {
            if (content is JsonElement jsonElement)
            {
                return jsonElement.GetProperty("data").GetString() ?? string.Empty;
            }
            
            var dataProp = content.GetType().GetProperty("Data");
            return dataProp?.GetValue(content)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// 尝试获取字符串属性，如果不存在返回 null
        /// </summary>
        public static string? TryGetString(object content, string propertyName)
        {
            try
            {
                if (content is JsonElement jsonElement)
                {
                    if (jsonElement.TryGetProperty(propertyName, out var prop))
                    {
                        return prop.GetString();
                    }
                    return null;
                }
                
                var prop2 = content.GetType().GetProperty(propertyName);
                return prop2?.GetValue(content)?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取布尔属性值
        /// </summary>
        public static bool GetBool(object content, string propertyName)
        {
            try
            {
                if (content is JsonElement jsonElement)
                {
                    if (jsonElement.TryGetProperty(propertyName, out var prop))
                    {
                        return prop.GetBoolean();
                    }
                    return false;
                }
                
                var prop2 = content.GetType().GetProperty(propertyName);
                var value = prop2?.GetValue(content);
                if (value is bool boolValue)
                {
                    return boolValue;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查 JSON 验证结果是否有效
        /// 支持多种格式：isValid, IsValid, valid 等
        /// </summary>
        public static bool IsJsonValidResult(object content)
        {
            // 首先获取文本内容
            var text = GetText(content);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            // 解析文本中的 JSON 并查找 isValid 字段
            try
            {
                var json = JsonDocument.Parse(text);
                
                // 尝试多种可能的属性名
                if (json.RootElement.TryGetProperty("isValid", out var isValidProp))
                {
                    return isValidProp.GetBoolean();
                }
                if (json.RootElement.TryGetProperty("IsValid", out var IsValidProp))
                {
                    return IsValidProp.GetBoolean();
                }
                if (json.RootElement.TryGetProperty("valid", out var validProp))
                {
                    return validProp.GetBoolean();
                }
                
                return false;
            }
            catch
            {
                // 如果不是 JSON，尝试直接从文本匹配
                return text.Contains("\"isValid\": true", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"isValid\":true", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"valid\": true", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"valid\":true", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 检查 JSON 验证结果是否无效
        /// </summary>
        public static bool IsJsonInvalidResult(object content)
        {
            var text = GetText(content);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            try
            {
                var json = JsonDocument.Parse(text);
                
                if (json.RootElement.TryGetProperty("isValid", out var isValidProp))
                {
                    return !isValidProp.GetBoolean();
                }
                if (json.RootElement.TryGetProperty("IsValid", out var IsValidProp))
                {
                    return !IsValidProp.GetBoolean();
                }
                if (json.RootElement.TryGetProperty("valid", out var validProp))
                {
                    return !validProp.GetBoolean();
                }
                
                return false;
            }
            catch
            {
                return text.Contains("\"isValid\": false", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"isValid\":false", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"valid\": false", StringComparison.OrdinalIgnoreCase) ||
                       text.Contains("\"valid\":false", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 将内容对象转换为 ImageContent
        /// </summary>
        public static ImageContent? GetImageContent(object content)
        {
            if (content is ImageContent imageContent)
            {
                return imageContent;
            }

            if (content is JsonElement jsonElement)
            {
                try
                {
                    return jsonElement.Deserialize<ImageContent>(JsonOptions);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 将内容对象转换为 AudioContent
        /// </summary>
        public static AudioContent? GetAudioContent(object content)
        {
            if (content is AudioContent audioContent)
            {
                return audioContent;
            }

            if (content is JsonElement jsonElement)
            {
                try
                {
                    return jsonElement.Deserialize<AudioContent>(JsonOptions);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 将内容对象转换为 VideoContent
        /// </summary>
        public static VideoContent? GetVideoContent(object content)
        {
            if (content is VideoContent videoContent)
            {
                return videoContent;
            }

            if (content is JsonElement jsonElement)
            {
                try
                {
                    return jsonElement.Deserialize<VideoContent>(JsonOptions);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 将内容对象转换为 TextContent
        /// </summary>
        public static TextContent? GetTextContent(object content)
        {
            if (content is TextContent textContent)
            {
                return textContent;
            }

            if (content is JsonElement jsonElement)
            {
                try
                {
                    return jsonElement.Deserialize<TextContent>(JsonOptions);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// 将内容对象转换为 EmbeddedResource
        /// </summary>
        public static EmbeddedResource? GetEmbeddedResource(object content)
        {
            if (content is EmbeddedResource embeddedResource)
            {
                return embeddedResource;
            }

            if (content is JsonElement jsonElement)
            {
                try
                {
                    return jsonElement.Deserialize<EmbeddedResource>(JsonOptions);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}

