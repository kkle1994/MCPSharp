using MCPSharp.Model.Results;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.Text;
using System.Text.Json;
using McpTextContent = MCPSharp.Model.Content.TextContent;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace MCPSharp.Test
{
    /// <summary>
    /// AI 模型集成测试 - 使用 OpenAI 兼容 API 进行完整的端到端测试
    /// </summary>
    [TestClass]
    public sealed class AIIntegrationTests
    {
        private static MCPClient? _mcpClient;
        private static IChatClient? _aiClient;
        private static IList<AIFunction>? _functions;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            // 初始化 MCP 客户端
            _mcpClient = new MCPClient("AI Integration Test Client", "1.0.0", "dotnet", TestConfiguration.TestSettings.ExampleServerPath);
            
            // 初始化 OpenAI 兼容客户端
            var openAiClient = new OpenAIClient(
                new ApiKeyCredential(TestConfiguration.OpenAI.ApiKey),
                new OpenAIClientOptions { Endpoint = new Uri(TestConfiguration.OpenAI.ApiUrl) }
            );
            
            // 使用 OpenAI ChatClient 的扩展方法获取 IChatClient
            _aiClient = openAiClient.GetChatClient(TestConfiguration.OpenAI.Model).AsChatClient();
            
            // 获取 MCP 工具作为 AI 函数
            _functions = await _mcpClient.GetFunctionsAsync();
            
            Console.WriteLine($"已加载 {_functions.Count} 个工具函数");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _mcpClient?.Dispose();
            (_aiClient as IDisposable)?.Dispose();
        }

        #region Basic AI Chat Tests

        [TestCategory("AI")]
        [TestMethod("AI基础对话测试")]
        public async Task TestAI_BasicChat()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请用一句话介绍自己")
            };

            var response = await _aiClient!.GetResponseAsync(messages);
            
            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrEmpty(response.Text));
            
            Console.WriteLine($"AI响应: {response.Text}");
        }

        #endregion

        #region Tool Discovery Tests

        [TestCategory("AI")]
        [TestMethod("AI工具发现测试")]
        public async Task TestAI_ToolDiscovery()
        {
            Assert.IsNotNull(_functions);
            Assert.IsTrue(_functions.Count > 0, "应该发现至少一个工具");
            
            foreach (var func in _functions)
            {
                Console.WriteLine($"工具: {func.Name}");
                Console.WriteLine($"  描述: {func.Description}");
                Console.WriteLine($"  Schema: {func.JsonSchema}");
                Console.WriteLine();
            }
            
            await Task.CompletedTask;
        }

        #endregion

        #region Tool Calling Tests

        [TestCategory("AI")]
        [TestMethod("AI调用Echo工具")]
        public async Task TestAI_CallEchoTool()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请使用Echo工具回显这段文字：'Hello from AI'")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
            
            // 检查是否有工具调用
            var toolCalls = response.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionCallContent>()
                .ToList();
            
            Console.WriteLine($"工具调用次数: {toolCalls.Count}");
            foreach (var call in toolCalls)
            {
                Console.WriteLine($"  调用工具: {call.Name}");
            }
        }

        [TestCategory("AI")]
        [TestMethod("AI调用数学计算工具")]
        public async Task TestAI_CallAddTool()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请使用Add工具计算 15 + 27 等于多少？")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
            
            // 响应应该包含 42
            Assert.IsTrue(response.Text.Contains("42") || response.Messages.Any(m => m.Text?.Contains("42") == true),
                "响应应该包含计算结果42");
        }

        [TestCategory("AI")]
        [TestMethod("AI调用哈希生成工具")]
        public async Task TestAI_CallHashTool()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请使用generate-hash工具对字符串'test'生成SHA256哈希值")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
        }

        [TestCategory("AI")]
        [TestMethod("AI生成随机数据")]
        public async Task TestAI_GenerateRandomData()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请使用generate-random-data工具生成5个GUID")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
        }

        #endregion

        #region Complex Workflow Tests

        [TestCategory("AI")]
        [TestMethod("AI复杂工作流 - 多工具组合")]
        public async Task TestAI_ComplexWorkflow()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, @"请完成以下任务：
1. 先使用Hello工具获取问候语
2. 然后使用Echo工具回显这个问候语
3. 最后用generate-hash工具对这个问候语生成MD5哈希

请依次执行这些操作并告诉我结果。")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
            
            // 记录所有工具调用
            var allMessages = response.Messages;
            foreach (var msg in allMessages)
            {
                foreach (var content in msg.Contents)
                {
                    if (content is FunctionCallContent funcCall)
                    {
                        Console.WriteLine($"工具调用: {funcCall.Name}({JsonSerializer.Serialize(funcCall.Arguments)})");
                    }
                    else if (content is FunctionResultContent funcResult)
                    {
                        Console.WriteLine($"工具结果: {funcResult.Result}");
                    }
                }
            }
        }

        [TestCategory("AI")]
        [TestMethod("AI JSON验证工作流")]
        public async Task TestAI_JsonValidationWorkflow()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, @"请使用validate-json工具验证以下JSON是否有效，并格式化输出：
{""users"":[{""name"":""张三"",""age"":25},{""name"":""李四"",""age"":30}]}")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
        }

        #endregion

        #region AIFunction Direct Invocation Tests

        [TestCategory("AI")]
        [TestMethod("直接调用AIFunction - Hello")]
        public async Task TestAIFunction_DirectInvoke_Hello()
        {
            var helloFunc = _functions!.FirstOrDefault(f => f.Name == "Hello");
            Assert.IsNotNull(helloFunc, "应该找到Hello函数");

            var result = await helloFunc.InvokeAsync();
            Assert.IsNotNull(result);

            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);

            var content = toolResult.Content.First();
            Assert.AreEqual("hello, claude.", TestHelpers.GetText(content));
        }

        [TestCategory("AI")]
        [TestMethod("直接调用AIFunction - Echo")]
        public async Task TestAIFunction_DirectInvoke_Echo()
        {
            var echoFunc = _functions!.FirstOrDefault(f => f.Name == "Echo");
            Assert.IsNotNull(echoFunc, "应该找到Echo函数");

            var args = new AIFunctionArguments { { "input", "测试输入" } };
            var result = await echoFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);

            var content = toolResult.Content.First();
            Assert.AreEqual("测试输入", TestHelpers.GetText(content));
        }

        [TestCategory("AI")]
        [TestMethod("直接调用AIFunction - Add")]
        public async Task TestAIFunction_DirectInvoke_Add()
        {
            var addFunc = _functions!.FirstOrDefault(f => f.Name == "Add");
            Assert.IsNotNull(addFunc, "应该找到Add函数");

            var args = new AIFunctionArguments { { "a", 100 }, { "b", 200 } };
            var result = await addFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);

            var content = toolResult.Content.First();
            Assert.AreEqual("300", TestHelpers.GetText(content));
        }

        #endregion

        #region Image Tool Tests

        [TestCategory("AI")]
        [TestMethod("AI调用图像创建工具")]
        public async Task TestAI_CreateImage()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请使用create-sample-image工具创建一个蓝色的示例图像")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
        }

        #endregion

        #region Error Handling Tests

        [TestCategory("AI")]
        [TestMethod("AI处理工具异常")]
        public async Task TestAI_HandleToolException()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请调用throw_exception工具")
            };

            var options = new ChatOptions
            {
                Tools = _functions!.ToList<AITool>()
            };

            var response = await _aiClient!.GetResponseAsync(messages, options);
            
            Assert.IsNotNull(response);
            Console.WriteLine($"AI响应: {response.Text}");
            
            // AI应该能够处理异常并给出合理的响应
        }

        #endregion

        #region Streaming Tests

        [TestCategory("AI")]
        [TestMethod("AI流式响应测试")]
        public async Task TestAI_StreamingResponse()
        {
            var messages = new List<AiChatMessage>
            {
                new(ChatRole.User, "请写一首关于编程的短诗（4行）")
            };

            var sb = new StringBuilder();
            
            await foreach (var update in _aiClient!.GetStreamingResponseAsync(messages))
            {
                if (!string.IsNullOrEmpty(update.Text))
                {
                    sb.Append(update.Text);
                    Console.Write(update.Text);
                }
            }
            
            Console.WriteLine();
            Assert.IsTrue(sb.Length > 0, "应该收到流式响应内容");
        }

        #endregion
    }
}
