using MCPSharp.Model.Content;
using MCPSharp.Model.Results;
using System.Text;
using System.Text.Json;

namespace MCPSharp.Test
{
    /// <summary>
    /// 内容类型测试 - 测试所有支持的内容类型
    /// </summary>
    [TestClass]
    public sealed class ContentTypeTests
    {
        private static MCPClient? _client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = new MCPClient("Content Type Test Client", "1.0.0", "dotnet", TestConfiguration.TestSettings.ExampleServerPath);
        }

        [ClassCleanup]
        public static void ClassCleanup() => _client?.Dispose();

        #region TextContent Tests

        [TestCategory("ContentType")]
        [TestMethod("TextContent - 基本文本内容")]
        public void TestTextContent_Basic()
        {
            var content = new TextContent("Hello, World!");
            
            Assert.AreEqual("text", content.Type);
            Assert.AreEqual("Hello, World!", content.Text);
        }

        [TestCategory("ContentType")]
        [TestMethod("TextContent - 中文内容")]
        public void TestTextContent_Chinese()
        {
            var content = new TextContent("你好，世界！");
            
            Assert.AreEqual("text", content.Type);
            Assert.AreEqual("你好，世界！", content.Text);
        }

        #endregion

        #region ImageContent Tests

        [TestCategory("ContentType")]
        [TestMethod("ImageContent - 基本图像内容")]
        public void TestImageContent_Basic()
        {
            var base64Data = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            var content = new ImageContent(base64Data, "image/png");
            
            Assert.AreEqual("image", content.Type);
            Assert.AreEqual(base64Data, content.Data);
            Assert.AreEqual("image/png", content.MimeType);
        }

        [TestCategory("ContentType")]
        [TestMethod("ImageContent - 通过工具返回图像")]
        public async Task TestImageContent_FromTool()
        {
            var result = await _client!.CallToolAsync("create-sample-image", new Dictionary<string, object> { { "color", "red" } });
            
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Content);
            
            // 使用 TestHelpers 来正确处理 JsonElement 反序列化
            var content = result.Content.First();
            var imageContent = TestHelpers.GetImageContent(content);
            Assert.IsNotNull(imageContent, "无法解析图像内容，可能是 JsonElement 或类型不匹配");
            Assert.AreEqual("image", imageContent.Type);
            Assert.AreEqual("image/png", imageContent.MimeType);
            Assert.IsFalse(string.IsNullOrEmpty(imageContent.Data));
            
            Console.WriteLine($"图像数据长度: {imageContent.Data.Length}");
        }

        [TestCategory("ContentType")]
        [TestMethod("ImageListResult - 图像列表结果")]
        public void TestImageListResult()
        {
            var images = new[]
            {
                new ImageContent("data1", "image/png"),
                new ImageContent("data2", "image/jpeg"),
                new ImageContent("data3", "image/gif")
            };
            
            var result = new ImageListResult(images);
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Content.Count());
        }

        #endregion

        #region AudioContent Tests

        [TestCategory("ContentType")]
        [TestMethod("AudioContent - 基本音频内容")]
        public void TestAudioContent_Basic()
        {
            var base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake audio data"));
            var content = new AudioContent(base64Data, "audio/mpeg");
            
            Assert.AreEqual("audio", content.Type);
            Assert.AreEqual(base64Data, content.Data);
            Assert.AreEqual("audio/mpeg", content.MimeType);
        }

        [TestCategory("ContentType")]
        [TestMethod("AudioContent - 不同音频格式")]
        public void TestAudioContent_DifferentFormats()
        {
            var formats = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/webm", "audio/aac" };
            var base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake audio data"));
            
            foreach (var format in formats)
            {
                var content = new AudioContent(base64Data, format);
                Assert.AreEqual("audio", content.Type);
                Assert.AreEqual(format, content.MimeType);
                Console.WriteLine($"测试格式: {format} - 通过");
            }
        }

        [TestCategory("ContentType")]
        [TestMethod("AudioListResult - 音频列表结果")]
        public void TestAudioListResult()
        {
            var audioItems = new[]
            {
                new AudioContent("audio1", "audio/mpeg"),
                new AudioContent("audio2", "audio/wav")
            };
            
            var result = new AudioListResult(audioItems);
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(2, result.Content.Count());
        }

        #endregion

        #region VideoContent Tests

        [TestCategory("ContentType")]
        [TestMethod("VideoContent - 基本视频内容")]
        public void TestVideoContent_Basic()
        {
            var base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake video data"));
            var content = new VideoContent(base64Data, "video/mp4");
            
            Assert.AreEqual("video", content.Type);
            Assert.AreEqual(base64Data, content.Data);
            Assert.AreEqual("video/mp4", content.MimeType);
        }

        [TestCategory("ContentType")]
        [TestMethod("VideoContent - 不同视频格式")]
        public void TestVideoContent_DifferentFormats()
        {
            var formats = new[] { "video/mp4", "video/webm", "video/ogg", "video/avi", "video/quicktime" };
            var base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("fake video data"));
            
            foreach (var format in formats)
            {
                var content = new VideoContent(base64Data, format);
                Assert.AreEqual("video", content.Type);
                Assert.AreEqual(format, content.MimeType);
                Console.WriteLine($"测试格式: {format} - 通过");
            }
        }

        [TestCategory("ContentType")]
        [TestMethod("VideoListResult - 视频列表结果")]
        public void TestVideoListResult()
        {
            var videoItems = new[]
            {
                new VideoContent("video1", "video/mp4"),
                new VideoContent("video2", "video/webm")
            };
            
            var result = new VideoListResult(videoItems);
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(2, result.Content.Count());
        }

        #endregion

        #region EmbeddedResource Tests

        [TestCategory("ContentType")]
        [TestMethod("ResourceContents - 文本资源")]
        public void TestResourceContents_Text()
        {
            var resource = ResourceContents.FromText("file:///path/to/file.txt", "文件内容", "text/plain");
            
            Assert.AreEqual("file:///path/to/file.txt", resource.Uri);
            Assert.AreEqual("文件内容", resource.Text);
            Assert.AreEqual("text/plain", resource.MimeType);
            Assert.IsNull(resource.Blob);
        }

        [TestCategory("ContentType")]
        [TestMethod("ResourceContents - 二进制资源")]
        public void TestResourceContents_Blob()
        {
            var blobData = Convert.ToBase64String(Encoding.UTF8.GetBytes("binary content"));
            var resource = ResourceContents.FromBlob("file:///path/to/file.bin", blobData, "application/octet-stream");
            
            Assert.AreEqual("file:///path/to/file.bin", resource.Uri);
            Assert.AreEqual(blobData, resource.Blob);
            Assert.AreEqual("application/octet-stream", resource.MimeType);
            Assert.IsNull(resource.Text);
        }

        [TestCategory("ContentType")]
        [TestMethod("EmbeddedResource - 从文本创建")]
        public void TestEmbeddedResource_FromText()
        {
            var embedded = EmbeddedResource.FromText("file:///doc.txt", "文档内容", "text/plain");
            
            Assert.AreEqual("resource", embedded.Type);
            Assert.IsNotNull(embedded.Resource);
            Assert.AreEqual("file:///doc.txt", embedded.Resource.Uri);
            Assert.AreEqual("文档内容", embedded.Resource.Text);
        }

        [TestCategory("ContentType")]
        [TestMethod("EmbeddedResource - 从二进制创建")]
        public void TestEmbeddedResource_FromBlob()
        {
            var blobData = Convert.ToBase64String(new byte[] { 0x00, 0x01, 0x02, 0x03 });
            var embedded = EmbeddedResource.FromBlob("file:///data.bin", blobData, "application/octet-stream");
            
            Assert.AreEqual("resource", embedded.Type);
            Assert.IsNotNull(embedded.Resource);
            Assert.AreEqual("file:///data.bin", embedded.Resource.Uri);
            Assert.AreEqual(blobData, embedded.Resource.Blob);
        }

        [TestCategory("ContentType")]
        [TestMethod("ResourceListResult - 资源列表结果")]
        public void TestResourceListResult()
        {
            var resources = new[]
            {
                EmbeddedResource.FromText("file:///a.txt", "A", "text/plain"),
                EmbeddedResource.FromText("file:///b.txt", "B", "text/plain"),
                EmbeddedResource.FromBlob("file:///c.bin", "AQID", "application/octet-stream")
            };
            
            var result = new ResourceListResult(resources);
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Content.Count());
        }

        #endregion

        #region MixedContentResult Tests

        [TestCategory("ContentType")]
        [TestMethod("MixedContentResult - 混合内容")]
        public void TestMixedContentResult_Basic()
        {
            var result = new MixedContentResult(
                new TextContent("这是文本"),
                new ImageContent("imagedata", "image/png"),
                new AudioContent("audiodata", "audio/mpeg")
            );
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(3, result.Content.Count());
        }

        [TestCategory("ContentType")]
        [TestMethod("MixedContentResult - 使用Builder构建")]
        public void TestMixedContentResult_Builder()
        {
            var result = MixedContentResult.Builder()
                .AddText("标题")
                .AddImage("imgdata", "image/jpeg")
                .AddAudio("audiodata", "audio/wav")
                .AddVideo("videodata", "video/mp4")
                .AddTextResource("file:///doc.md", "# 文档", "text/markdown")
                .Build();
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(5, result.Content.Count());
            
            var contentTypes = result.Content.Cast<IContent>().Select(c => c.Type).ToList();
            Assert.IsTrue(contentTypes.Contains("text"));
            Assert.IsTrue(contentTypes.Contains("image"));
            Assert.IsTrue(contentTypes.Contains("audio"));
            Assert.IsTrue(contentTypes.Contains("video"));
            Assert.IsTrue(contentTypes.Contains("resource"));
        }

        [TestCategory("ContentType")]
        [TestMethod("MixedContentResult - 空内容")]
        public void TestMixedContentResult_Empty()
        {
            var result = new MixedContentResult();
            
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(0, result.Content.Count());
        }

        #endregion

        #region JSON Serialization Tests

        [TestCategory("Serialization")]
        [TestMethod("内容类型JSON序列化")]
        public void TestContentType_JsonSerialization()
        {
            var contents = new IContent[]
            {
                new TextContent("text content"),
                new ImageContent("imgdata", "image/png"),
                new AudioContent("audiodata", "audio/mpeg"),
                new VideoContent("videodata", "video/mp4"),
                EmbeddedResource.FromText("file:///test.txt", "file content", "text/plain")
            };

            foreach (var content in contents)
            {
                var json = JsonSerializer.Serialize(content, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });
                
                Assert.IsFalse(string.IsNullOrEmpty(json));
                Assert.IsTrue(json.Contains($"\"type\": \"{content.Type}\"") || 
                             json.Contains($"\"type\":\"{content.Type}\""));
                
                Console.WriteLine($"{content.Type}:\n{json}\n");
            }
        }

        #endregion
    }
}

