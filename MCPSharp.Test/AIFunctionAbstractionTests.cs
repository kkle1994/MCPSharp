using MCPSharp.Model.Content;
using MCPSharp.Model.Results;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace MCPSharp.Test
{
    /// <summary>
    /// AI 函数抽象层测试 - 测试 MCPClient.GetFunctionsAsync() 返回的 AIFunction
    /// </summary>
    [TestClass]
    public class AIFunctionAbstractionTests
    {
        private static MCPClient? _client;
        private static IList<AIFunction>? _functions;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _client = new MCPClient("AIFunction Test Client", "1.0.0", "dotnet", TestConfiguration.TestSettings.ExampleServerPath);
            _functions = await _client.GetFunctionsAsync();
            
            Console.WriteLine($"已加载 {_functions.Count} 个 AIFunction");
        }

        [ClassCleanup]
        public static void ClassCleanup() => _client?.Dispose();

        #region Function Discovery Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction - 函数发现")]
        public void TestAIFunction_Discovery()
        {
            Assert.IsNotNull(_functions);
            Assert.IsTrue(_functions.Count > 0, "应该发现至少一个函数");
            
            foreach (var func in _functions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(func.Name), "函数名称不应为空");
                Console.WriteLine($"函数: {func.Name}");
                Console.WriteLine($"  描述: {func.Description}");
            }
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction - Schema生成")]
        public void TestAIFunction_Schema()
        {
            Assert.IsNotNull(_functions);
            
            foreach (var func in _functions)
            {
                var schema = func.JsonSchema;
                Assert.IsNotNull(schema, $"函数 {func.Name} 应该有 JsonSchema");
                
                Console.WriteLine($"函数: {func.Name}");
                Console.WriteLine($"  Schema: {schema}");
                Console.WriteLine();
            }
        }

        #endregion

        #region Direct Invocation Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 无参数函数")]
        public async Task TestAIFunction_InvokeNoParams()
        {
            var helloFunc = _functions!.FirstOrDefault(f => f.Name == "Hello");
            Assert.IsNotNull(helloFunc, "应该找到 Hello 函数");

            var result = await helloFunc.InvokeAsync();
            
            Assert.IsNotNull(result);
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError, $"调用不应出错");
            
            var content = toolResult.Content.First();
            Assert.AreEqual("hello, claude.", TestHelpers.GetText(content));
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 带参数函数")]
        public async Task TestAIFunction_InvokeWithParams()
        {
            var echoFunc = _functions!.FirstOrDefault(f => f.Name == "Echo");
            Assert.IsNotNull(echoFunc, "应该找到 Echo 函数");

            var args = new AIFunctionArguments { { "input", "Hello AIFunction" } };
            var result = await echoFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("Hello AIFunction", TestHelpers.GetText(content));
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 多参数函数")]
        public async Task TestAIFunction_InvokeMultipleParams()
        {
            var addFunc = _functions!.FirstOrDefault(f => f.Name == "Add");
            Assert.IsNotNull(addFunc, "应该找到 Add 函数");

            var args = new AIFunctionArguments { { "a", 50 }, { "b", 75 } };
            var result = await addFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("125", TestHelpers.GetText(content));
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 动态工具")]
        public async Task TestAIFunction_InvokeDynamicTool()
        {
            var dynamicFunc = _functions!.FirstOrDefault(f => f.Name == "dynamicTool");
            Assert.IsNotNull(dynamicFunc, "应该找到 dynamicTool 函数");

            var args = new AIFunctionArguments 
            { 
                { "input", "first value" }, 
                { "input2", "second value" } 
            };
            var result = await dynamicFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("hello, first value.\nsecond value", TestHelpers.GetText(content));
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 哈希生成")]
        public async Task TestAIFunction_InvokeHashGenerator()
        {
            var hashFunc = _functions!.FirstOrDefault(f => f.Name == "generate-hash");
            Assert.IsNotNull(hashFunc, "应该找到 generate-hash 函数");

            var args = new AIFunctionArguments 
            { 
                { "input", "test" }, 
                { "algorithm", "MD5" } 
            };
            var result = await hashFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            // MD5 of "test" = 098f6bcd4621d373cade4e832627b4f6
            Assert.AreEqual("098f6bcd4621d373cade4e832627b4f6", TestHelpers.GetText(content));
        }

        #endregion

        #region Image Function Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 图像创建")]
        public async Task TestAIFunction_InvokeImageCreation()
        {
            var imageFunc = _functions!.FirstOrDefault(f => f.Name == "create-sample-image");
            Assert.IsNotNull(imageFunc, "应该找到 create-sample-image 函数");

            var args = new AIFunctionArguments { { "color", "blue" } };
            var result = await imageFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("image", TestHelpers.GetType(content));
            Assert.AreEqual("image/png", TestHelpers.GetMimeType(content));
            Assert.IsFalse(string.IsNullOrEmpty(TestHelpers.GetData(content)));
            
            Console.WriteLine($"图像数据长度: {TestHelpers.GetData(content).Length}");
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 图像处理")]
        public async Task TestAIFunction_InvokeImageProcessing()
        {
            var processFunc = _functions!.FirstOrDefault(f => f.Name == "process-image");
            Assert.IsNotNull(processFunc, "应该找到 process-image 函数");

            var testImageData = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
            
            var args = new AIFunctionArguments 
            { 
                { "imageData", testImageData }, 
                { "mimeType", "image/png" } 
            };
            var result = await processFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("image/png", TestHelpers.GetMimeType(content));
        }

        #endregion

        #region Complex Object Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 复杂对象参数")]
        public async Task TestAIFunction_InvokeComplexObject()
        {
            var complexFunc = _functions!.FirstOrDefault(f => f.Name == "AddComplex");
            Assert.IsNotNull(complexFunc, "应该找到 AddComplex 函数");

            var complexObj = new 
            { 
                Name = "测试用户", 
                Age = 30, 
                Hobbies = new[] { "编程", "阅读", "运动" } 
            };
            
            var args = new AIFunctionArguments { { "obj", complexObj } };
            var result = await complexFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.AreEqual("Name: 测试用户, Age: 30, Hobbies: 编程, 阅读, 运动", TestHelpers.GetText(content));
        }

        #endregion

        #region Error Handling Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 异常处理")]
        public async Task TestAIFunction_HandleException()
        {
            var exceptionFunc = _functions!.FirstOrDefault(f => f.Name == "throw_exception");
            Assert.IsNotNull(exceptionFunc, "应该找到 throw_exception 函数");

            var result = await exceptionFunc.InvokeAsync();
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsTrue(toolResult.IsError, "调用应该返回错误");
            
            var content = toolResult.Content.First();
            Assert.AreEqual("This is an exception", TestHelpers.GetText(content));
        }

        #endregion

        #region JSON Validation Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - JSON验证")]
        public async Task TestAIFunction_InvokeJsonValidation()
        {
            var jsonFunc = _functions!.FirstOrDefault(f => f.Name == "validate-json");
            Assert.IsNotNull(jsonFunc, "应该找到 validate-json 函数");

            var validJson = "{\"key\": \"value\", \"number\": 123}";
            var args = new AIFunctionArguments 
            { 
                { "jsonString", validJson }, 
                { "indent", true } 
            };
            var result = await jsonFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.IsTrue(TestHelpers.IsJsonValidResult(content), $"期望有效的 JSON 验证结果，实际文本: {TestHelpers.GetText(content)}");
            
            Console.WriteLine($"验证结果: {TestHelpers.GetText(content)}");
        }

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 无效JSON验证")]
        public async Task TestAIFunction_InvokeInvalidJsonValidation()
        {
            var jsonFunc = _functions!.FirstOrDefault(f => f.Name == "validate-json");
            Assert.IsNotNull(jsonFunc, "应该找到 validate-json 函数");

            var invalidJson = "{invalid json}";
            var args = new AIFunctionArguments 
            { 
                { "jsonString", invalidJson }, 
                { "indent", true } 
            };
            var result = await jsonFunc.InvokeAsync(args);
            
            var toolResult = result as CallToolResult;
            Assert.IsNotNull(toolResult);
            Assert.IsFalse(toolResult.IsError);
            
            var content = toolResult.Content.First();
            Assert.IsTrue(TestHelpers.IsJsonInvalidResult(content), $"期望无效的 JSON 验证结果，实际文本: {TestHelpers.GetText(content)}");
            
            Console.WriteLine($"验证结果: {TestHelpers.GetText(content)}");
        }

        #endregion

        #region Batch Invocation Tests

        [TestCategory("AIFunctions")]
        [TestMethod("AIFunction调用 - 批量调用")]
        public async Task TestAIFunction_BatchInvoke()
        {
            var echoFunc = _functions!.FirstOrDefault(f => f.Name == "Echo");
            Assert.IsNotNull(echoFunc);

            var tasks = new List<ValueTask<object?>>();
            for (int i = 0; i < 10; i++)
            {
                var args = new AIFunctionArguments { { "input", $"Message {i}" } };
                tasks.Add(echoFunc.InvokeAsync(args));
            }

            // 等待所有任务完成
            var results = new List<object?>();
            foreach (var task in tasks)
            {
                results.Add(await task);
            }
            
            Assert.AreEqual(10, results.Count);
            for (int i = 0; i < results.Count; i++)
            {
                var toolResult = results[i] as CallToolResult;
                Assert.IsNotNull(toolResult);
                Assert.IsFalse(toolResult.IsError);
                
                var content = toolResult.Content.First();
                Assert.AreEqual($"Message {i}", TestHelpers.GetText(content));
            }
        }

        #endregion
    }
}
