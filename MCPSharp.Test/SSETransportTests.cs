namespace MCPSharp.Test
{
    /// <summary>
    /// SSE (Server-Sent Events) 传输测试
    /// 注意: SSE 传输目前处于实验阶段
    /// </summary>
    [TestClass]
    public sealed class SSETransportTests
    {
        private static MCPClient? _stdioClient;
        private static MCPClient? _sseClient;
        private static bool _sseAvailable = false;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // SSE 测试目前被禁用，因为 SSE 传输还在开发中
            // 取消下面的注释以启用 SSE 测试
            
            // try
            // {
            //     // 启动一个 MCP 服务器实例
            //     _stdioClient = new MCPClient("SSE Test Server", "1.0.0", "MCPSharp.Example.exe");
            //     
            //     // 连接到 SSE 端点
            //     _sseClient = new MCPClient(new Uri("http://localhost:8000/sse"), "SSE Test Client", "1.0.0");
            //     _sseAvailable = true;
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"SSE 服务器不可用: {ex.Message}");
            //     _sseAvailable = false;
            // }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _sseClient?.Dispose();
            _stdioClient?.Dispose();
        }

        private void EnsureSSEAvailable()
        {
            if (!_sseAvailable)
            {
                Assert.Inconclusive("SSE 传输不可用，跳过测试");
            }
        }

        #region SSE Connection Tests

        [TestCategory("SSE")]
        [TestMethod("SSE - 连接测试")]
        public void TestSSE_Connection()
        {
            EnsureSSEAvailable();
            
            Assert.IsTrue(_sseClient!.Initialized, "SSE 客户端应该已初始化");
        }

        [TestCategory("SSE")]
        [TestMethod("SSE - 列出工具")]
        public async Task TestSSE_ListTools()
        {
            EnsureSSEAvailable();
            
            var tools = await _sseClient!.GetToolsAsync();
            
            Assert.IsNotNull(tools);
            Assert.IsTrue(tools.Count > 0, "应该至少有一个工具");
            
            tools.ForEach(tool =>
            {
                Assert.IsFalse(string.IsNullOrEmpty(tool.Name));
                Assert.IsFalse(string.IsNullOrEmpty(tool.Description));
                Console.WriteLine($"工具: {tool.Name}");
            });
        }

        [TestCategory("SSE")]
        [TestMethod("SSE - 调用工具")]
        public async Task TestSSE_CallTool()
        {
            EnsureSSEAvailable();
            
            var result = await _sseClient!.CallToolAsync("Hello");
            
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Content);
        }

        #endregion

        #region SSE Tool Tests (Placeholder)

        public class TestTool
        {
            [McpTool("sse-test", "SSE test function")]
            public static async Task<string> TestFunctionAsync() => await Task.FromResult("SSE test success");
        }

        #endregion
    }
}
