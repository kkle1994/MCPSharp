using MCPSharp.Example;
using MCPSharp.Model.Content;
using MCPSharp.Model.Results;

namespace MCPSharp.Test
{
    /// <summary>
    /// STDIO 传输协议测试
    /// </summary>
    [TestClass]
    public sealed class STDIOTransportTests
    {
        private static MCPClient? _client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _client = new MCPClient("STDIO Test Client", "1.0.0", "dotnet", TestConfiguration.TestSettings.ExampleServerPath);
        }

        [ClassCleanup]
        public static void ClassCleanup() => _client?.Dispose();

        #region Tools List Tests

        [TestCategory("Tools")]
        [TestMethod("列出所有工具")]
        public async Task Test_ListTools()
        {
            var tools = await _client!.GetToolsAsync();
            
            Assert.IsNotNull(tools);
            Assert.IsTrue(tools.Count > 0, "应该至少有一个工具");
            
            tools.ForEach(tool =>
            {
                Assert.IsFalse(string.IsNullOrEmpty(tool.Name), "工具名称不应为空");
                Assert.IsFalse(string.IsNullOrEmpty(tool.Description), "工具描述不应为空");
                Console.WriteLine($"工具: {tool.Name} - {tool.Description}");
            });
        }

        #endregion

        #region Tool Call Tests

        [TestCategory("Tools")]
        [TestMethod("调用无参工具 - Hello")]
        public async Task TestCallTool_Hello()
        {
            var result = await _client!.CallToolAsync("Hello");
            
            Assert.IsFalse(result.IsError, "工具调用不应返回错误");
            Assert.IsNotNull(result.Content);
            
            var content = result.Content.First();
            Assert.AreEqual("hello, claude.", TestHelpers.GetText(content));
            Assert.AreEqual("text", TestHelpers.GetType(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用带参数工具 - Echo")]
        public async Task TestCallTool_Echo()
        {
            const string testMessage = "这是一个测试消息";
            var result = await _client!.CallToolAsync("Echo", new Dictionary<string, object> { { "input", testMessage } });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Assert.AreEqual(testMessage, TestHelpers.GetText(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用数学工具 - Add")]
        public async Task TestCallTool_Add()
        {
            var result = await _client!.CallToolAsync("Add", new Dictionary<string, object> { { "a", 10 }, { "b", 20 } });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Assert.AreEqual("30", TestHelpers.GetText(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用动态创建的工具")]
        public async Task TestCallTool_Dynamic()
        {
            var result = await _client!.CallToolAsync("dynamicTool", new Dictionary<string, object> 
            { 
                { "input", "test string" }, 
                { "input2", "another string" } 
            });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Assert.AreEqual("hello, test string.\nanother string", TestHelpers.GetText(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用复杂对象工具")]
        public async Task TestCallTool_ComplexObject()
        {
            var complexObj = new ComplicatedObject 
            { 
                Name = "张三", 
                Age = 25, 
                Hobbies = ["编程", "游戏", "阅读"] 
            };
            
            var result = await _client!.CallToolAsync("AddComplex", new Dictionary<string, object> { { "obj", complexObj } });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Assert.AreEqual("Name: 张三, Age: 25, Hobbies: 编程, 游戏, 阅读", TestHelpers.GetText(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用哈希生成工具")]
        public async Task TestCallTool_GenerateHash()
        {
            var result = await _client!.CallToolAsync("generate-hash", new Dictionary<string, object> 
            { 
                { "input", "hello world" }, 
                { "algorithm", "SHA256" } 
            });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            // SHA256 of "hello world" 
            Assert.AreEqual("b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9", TestHelpers.GetText(content));
        }

        [TestCategory("Tools")]
        [TestMethod("调用JSON验证工具")]
        public async Task TestCallTool_ValidateJson()
        {
            var validJson = "{\"name\": \"test\", \"value\": 123}";
            var result = await _client!.CallToolAsync("validate-json", new Dictionary<string, object> 
            { 
                { "jsonString", validJson }, 
                { "indent", true } 
            });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Assert.IsTrue(TestHelpers.IsJsonValidResult(content), $"期望有效的 JSON 验证结果，实际文本: {TestHelpers.GetText(content)}");
        }

        [TestCategory("Tools")]
        [TestMethod("调用随机数据生成工具")]
        public async Task TestCallTool_GenerateRandomData()
        {
            var result = await _client!.CallToolAsync("generate-random-data", new Dictionary<string, object> 
            { 
                { "dataType", "guid" }, 
                { "count", 3 } 
            });
            
            Assert.IsFalse(result.IsError);
            var content = result.Content.First();
            Console.WriteLine($"生成的GUID: {TestHelpers.GetText(content)}");
        }

        #endregion

        #region Error Handling Tests

        [TestCategory("Errors")]
        [TestMethod("异常处理测试")]
        public async Task TestCallTool_Exception()
        {
            var result = await _client!.CallToolAsync("throw_exception");
            
            Assert.IsTrue(result.IsError);
            var content = result.Content.First();
            Assert.AreEqual("This is an exception", TestHelpers.GetText(content));
        }

        [TestCategory("Errors")]
        [TestMethod("调用不存在的工具")]
        public async Task TestCallTool_InvalidTool()
        {
            var result = await _client!.CallToolAsync("NonExistentTool");
            Assert.IsTrue(result.IsError, "调用不存在的工具应返回错误");
        }

        [TestCategory("Errors")]
        [TestMethod("调用缺少必需参数的工具")]
        public async Task TestCallTool_MissingRequiredParameter()
        {
            var result = await _client!.CallToolAsync("Echo", new Dictionary<string, object>());
            Assert.IsTrue(result.IsError, "缺少必需参数应返回错误");
        }

        #endregion

        #region Resources Tests

        [TestCategory("Resources")]
        [TestMethod("列出资源")]
        public async Task TestResources_List()
        {
            var result = await _client!.GetResourcesAsync();
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Resources.Count > 0, "应该至少有一个资源");
            
            result.Resources.ForEach(resource =>
            {
                Console.WriteLine($"资源: {resource.Name} - {resource.Uri}");
            });
        }

        #endregion

        #region Prompts Tests

        [TestCategory("Prompts")]
        [TestMethod("列出提示")]
        public async Task TestPrompts_List()
        {
            var result = await _client!.GetPromptListAsync();
            Assert.IsNotNull(result);
            Console.WriteLine($"提示数量: {result.Prompts.Count}");
        }

        #endregion

        #region Misc Tests

        [TestCategory("Misc")]
        [TestMethod("Ping测试")]
        public async Task TestPing()
        {
            var result = await _client!.SendPingAsync();
            Assert.IsNotNull(result);
        }

        #endregion
    }
}
