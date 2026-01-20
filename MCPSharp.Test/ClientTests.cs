using System.Runtime.InteropServices;
using MCPSharp.Model.Content;

namespace MCPSharp.Test
{
    /// <summary>
    /// MCP 客户端测试 - 使用外部 MCP 服务器
    /// </summary>
    [TestClass]
    public class ClientTests
    {
        private static MCPClient? _client;
        private static bool _serverAvailable = false;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            try
            {
                // 尝试连接到 @modelcontextprotocol/server-everything
                var npxPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                    ? "c:\\program files\\nodejs\\npx.cmd" 
                    : "npx";
                
                _client = new MCPClient("External Server Test Client", "1.0.0", npxPath, "-y @modelcontextprotocol/server-everything")
                {
                    GetPermission = (Dictionary<string, object> parameters) => true
                };
                
                _serverAvailable = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"外部服务器不可用: {ex.Message}");
                _serverAvailable = false;
            }
        }

        [ClassCleanup]
        public static void ClassCleanup() => _client?.Dispose();

        private void EnsureServerAvailable()
        {
            if (!_serverAvailable)
            {
                Assert.Inconclusive("外部MCP服务器不可用，跳过测试");
            }
        }

        #region Tools Tests

        [TestCategory("ExternalServer")]
        [TestMethod("外部服务器 - 列出工具")]
        public async Task TestExternalServer_ListTools()
        {
            EnsureServerAvailable();
            
            var tools = await _client!.GetToolsAsync();
            
            Assert.IsNotNull(tools);
            Assert.IsTrue(tools.Count > 0, "外部服务器应该提供工具");
            
            tools.ForEach(tool =>
            {
                Assert.IsFalse(string.IsNullOrEmpty(tool.Name));
                Assert.IsFalse(string.IsNullOrEmpty(tool.Description));
                Console.WriteLine($"工具: {tool.Name} - {tool.Description}");
            });
        }

        [TestCategory("ExternalServer")]
        [TestMethod("外部服务器 - 调用Echo工具")]
        public async Task TestExternalServer_CallEchoTool()
        {
            EnsureServerAvailable();
            
            var result = await _client!.CallToolAsync("echo", new Dictionary<string, object> { { "message", "Hello MCP" } });
            
            Assert.IsFalse(result.IsError);
            var textContent = result.Content.First() as TextContent;
            Assert.IsNotNull(textContent);
            Assert.AreEqual("Echo: Hello MCP", textContent.Text);
        }

        #endregion

        #region Prompts Tests

        [TestCategory("ExternalServer")]
        [TestMethod("外部服务器 - 列出提示")]
        public async Task TestExternalServer_ListPrompts()
        {
            EnsureServerAvailable();
            
            var result = await _client!.GetPromptListAsync();
            Assert.IsNotNull(result);
            Console.WriteLine($"提示数量: {result.Prompts.Count}");
        }

        #endregion

        #region Resources Tests

        [TestCategory("ExternalServer")]
        [TestMethod("外部服务器 - 列出资源")]
        public async Task TestExternalServer_ListResources()
        {
            EnsureServerAvailable();
            
            var result = await _client!.GetResourcesAsync();
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Resources.Count > 0, "外部服务器应该提供资源");
            
            result.Resources.ForEach(resource =>
            {
                Console.WriteLine($"资源: {resource.Name} - {resource.Uri}");
            });
        }

        #endregion

        #region Permission Tests

        [TestCategory("Permission")]
        [TestMethod("权限控制 - 拒绝工具调用")]
        public async Task TestPermission_DenyToolCall()
        {
            EnsureServerAvailable();
            
            // 临时设置权限检查为拒绝
            var originalPermission = _client!.GetPermission;
            _client.GetPermission = (parameters) => false;
            
            try
            {
                var result = await _client.CallToolAsync("echo", new Dictionary<string, object> { { "message", "test" } });
                
                Assert.IsTrue(result.IsError);
                var textContent = result.Content.First() as TextContent;
                Assert.IsNotNull(textContent);
                Assert.AreEqual("Permission Denied.", textContent.Text);
            }
            finally
            {
                // 恢复原始权限设置
                _client.GetPermission = originalPermission;
            }
        }

        [TestCategory("Permission")]
        [TestMethod("权限控制 - 允许工具调用")]
        public async Task TestPermission_AllowToolCall()
        {
            EnsureServerAvailable();
            
            // 确保权限检查为允许
            _client!.GetPermission = (parameters) => true;
            
            var result = await _client.CallToolAsync("echo", new Dictionary<string, object> { { "message", "allowed" } });
            
            Assert.IsFalse(result.IsError);
        }

        #endregion
    }
}
