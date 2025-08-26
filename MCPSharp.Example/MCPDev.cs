using MCPSharp.Model.Content;
using MCPSharp.Model.Results;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Security.Cryptography;

namespace MCPSharp.Example
{
    ///<summary>testing interface for custom .net mcp server</summary>
    public class MCPDev()
    {
        [McpResource("name", "test://{name}")]
        public string Name(string name) => $"hello {name}";


        [McpResource("settings", "test://settings", "string", "the settings document")]
        public string Settings { get; set; } = "settings";


        [McpTool("write-to-console", "write a string to the console")] 
        public static void WriteToConsole(string message) => Console.WriteLine(message);

        ///<summary>just returns a message for testing.</summary>
        [McpTool] 
        public static string Hello() => "hello, claude.";

        ///<summary>returns ths input string back</summary>
        ///<param name="input">the string to echo</param>
        [McpTool]
        public static string Echo([McpParameter(true)] string input) => input;

        ///<summary>Add Two Numbers</summary>
        ///<param name="a">first number</param>
        ///<param name="b">second number</param>
        [McpTool] 
        public static string Add(int a, int b) => (a + b).ToString();


        /// <summary>
        /// Adds a complex object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [McpTool]
        public static string AddComplex(ComplicatedObject obj) => $"Name: {obj.Name}, Age: {obj.Age}, Hobbies: {string.Join(", ", obj.Hobbies)}";

        /// <summary>
        /// throws an exception - for ensuring we handle them gracefully
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [McpFunction("throw_exception")] //leaving this one as [McpFunction] for testing purposes
        public static string Exception() => throw new Exception("This is an exception");

        /// <summary>
        /// 处理图像内容并返回图像列表结果
        /// </summary>
        /// <param name="imageData">Base64编码的图像数据</param>
        /// <param name="mimeType">图像的MIME类型</param>
        /// <returns>处理后的图像列表</returns>
        [McpTool("process-image", "Process an image and return image list result")]
        public static ImageListResult ProcessImage(
            [McpParameter(true, "Base64 encoded image data")] string imageData,
            [McpParameter(true, "MIME type of the image")] string mimeType)
        {
            // 创建图像内容对象
            var processedImage = new ImageContent(imageData, mimeType);
            
            // 可以在这里添加实际的图像处理逻辑
            // 例如：调整大小、应用滤镜等
            
            return new ImageListResult(new[] { processedImage });
        }

        /// <summary>
        /// 创建一个示例图像（简单的1x1像素PNG）
        /// </summary>
        /// <param name="color">颜色名称（red, green, blue, black, white）</param>
        /// <returns>包含图像的结果</returns>
        [McpTool("create-sample-image", "Create a sample 1x1 pixel image with specified color")]
        public static ImageListResult CreateSampleImage(
            [McpParameter(true, "Color name: red, green, blue, black, white")] string color)
        {
            // 简单的1x1像素PNG的base64数据
            var colorData = color.ToLower() switch
            {
                "red" => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==",
                "green" => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
                "blue" => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChAGA4GZuQwAAAABJRU5ErkJggg==",
                "black" => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
                "white" => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP///8/AAVfAn8BPh+VAAAAABJRU5ErkJggg==",
                _ => "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==" // default to black
            };
            
            var imageContent = new ImageContent(colorData, "image/png");
            return new ImageListResult(new[] { imageContent });
        }

        /// <summary>
        /// 处理复杂对象列表并进行数据分析
        /// </summary>
        /// <param name="objects">复杂对象数组</param>
        /// <returns>分析结果的JSON字符串</returns>
        [McpTool("analyze-complex-objects", "Analyze an array of complex objects and return statistics")]
        public static string AnalyzeComplexObjects(
            [McpParameter(true, "Array of complex objects to analyze")] ComplicatedObject[] objects)
        {
            if (objects == null || objects.Length == 0)
                return "{\"error\": \"No objects provided\"}";

            var analysis = new
            {
                TotalCount = objects.Length,
                AverageAge = objects.Average(o => o.Age),
                MinAge = objects.Min(o => o.Age),
                MaxAge = objects.Max(o => o.Age),
                UniqueNames = objects.Select(o => o.Name).Distinct().Count(),
                MostCommonHobbies = objects
                    .SelectMany(o => o.Hobbies)
                    .GroupBy(h => h)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AgeGroups = new
                {
                    Children = objects.Count(o => o.Age < 18),
                    Adults = objects.Count(o => o.Age >= 18 && o.Age < 65),
                    Seniors = objects.Count(o => o.Age >= 65)
                }
            };

            return JsonSerializer.Serialize(analysis, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// 生成加密哈希值
        /// </summary>
        /// <param name="input">要哈希的字符串</param>
        /// <param name="algorithm">哈希算法（MD5, SHA1, SHA256, SHA512）</param>
        /// <returns>十六进制哈希值</returns>
        [McpTool("generate-hash", "Generate cryptographic hash of input string")]
        public static string GenerateHash(
            [McpParameter(true, "Input string to hash")] string input,
            [McpParameter(false, "Hash algorithm: MD5, SHA1, SHA256, SHA512")] string algorithm = "SHA256")
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash;

                switch (algorithm.ToUpper())
                {
                    case "MD5":
                        hash = MD5.HashData(bytes);
                        break;
                    case "SHA1":
                        hash = SHA1.HashData(bytes);
                        break;
                    case "SHA256":
                        hash = SHA256.HashData(bytes);
                        break;
                    case "SHA512":
                        hash = SHA512.HashData(bytes);
                        break;
                    default:
                        return $"{{\"error\": \"Unsupported algorithm: {algorithm}\"}";
                }

                return Convert.ToHexString(hash).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"{ex.Message}\"}}";
            }
        }

        /// <summary>
        /// 生成随机数据
        /// </summary>
        /// <param name="dataType">数据类型：guid, number, string, boolean</param>
        /// <param name="count">生成数量</param>
        /// <param name="options">选项（对于number: min-max, 对于string: length）</param>
        /// <returns>生成的随机数据的JSON数组</returns>
        [McpTool("generate-random-data", "Generate random data of specified type and count")]
        public static string GenerateRandomData(
            [McpParameter(true, "Data type: guid, number, string, boolean")] string dataType,
            [McpParameter(false, "Number of items to generate")] int count = 1,
            [McpParameter(false, "Options (e.g., \"1-100\" for numbers, \"10\" for string length)")] string options = "")
        {
            try
            {
                var random = new Random();
                var results = new List<object>();

                for (int i = 0; i < Math.Min(count, 100); i++) // 限制最多100个
                {
                    switch (dataType.ToLower())
                    {
                        case "guid":
                            results.Add(Guid.NewGuid().ToString());
                            break;
                        case "number":
                            var range = ParseNumberOptions(options);
                            results.Add(random.Next(range.min, range.max + 1));
                            break;
                        case "string":
                            var length = ParseStringLength(options);
                            results.Add(GenerateRandomString(random, length));
                            break;
                        case "boolean":
                            results.Add(random.NextDouble() > 0.5);
                            break;
                        default:
                            return $"{{\"error\": \"Unsupported data type: {dataType}\"}";
                    }
                }

                return JsonSerializer.Serialize(results, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"{ex.Message}\"}}";
            }
        }

        /// <summary>
        /// 验证和格式化JSON字符串
        /// </summary>
        /// <param name="jsonString">要验证的JSON字符串</param>
        /// <param name="indent">是否格式化输出</param>
        /// <returns>验证结果和格式化的JSON</returns>
        [McpTool("validate-json", "Validate and format JSON string")]
        public static string ValidateJson(
            [McpParameter(true, "JSON string to validate")] string jsonString,
            [McpParameter(false, "Format output with indentation")] bool indent = true)
        {
            try
            {
                var document = JsonDocument.Parse(jsonString);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = indent
                };
                
                var formatted = JsonSerializer.Serialize(document, options);
                
                return JsonSerializer.Serialize(new
                {
                    IsValid = true,
                    Message = "JSON is valid",
                    FormattedJson = formatted
                }, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (JsonException ex)
            {
                return JsonSerializer.Serialize(new
                {
                    IsValid = false,
                    Message = $"JSON validation failed: {ex.Message}",
                    FormattedJson = (string?)null
                }, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        // 辅助方法
        private static (int min, int max) ParseNumberOptions(string options)
        {
            if (string.IsNullOrEmpty(options))
                return (1, 100);

            var parts = options.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                return (min, max);

            return (1, 100);
        }

        private static int ParseStringLength(string options)
        {
            if (int.TryParse(options, out int length) && length > 0 && length <= 1000)
                return length;
            return 8; // 默认长度
        }

        private static string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}