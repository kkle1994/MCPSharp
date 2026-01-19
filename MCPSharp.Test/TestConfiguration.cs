using Microsoft.Extensions.Configuration;

namespace MCPSharp.Test
{
    /// <summary>
    /// 测试配置管理类
    /// </summary>
    public static class TestConfiguration
    {
        private static IConfiguration? _configuration;
        
        /// <summary>
        /// 获取配置实例
        /// </summary>
        public static IConfiguration Configuration
        {
            get
            {
                _configuration ??= new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                return _configuration;
            }
        }

        /// <summary>
        /// OpenAI 配置
        /// </summary>
        public static class OpenAI
        {
            public static string ApiUrl => Configuration["OpenAI:ApiUrl"] ?? throw new InvalidOperationException("OpenAI:ApiUrl not configured");
            public static string ApiKey => Configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey not configured");
            public static string Model => Configuration["OpenAI:Model"] ?? "gpt-4";
        }

        /// <summary>
        /// 测试设置
        /// </summary>
        public static class TestSettings
        {
            public static string ExampleServerPath => Configuration["TestSettings:ExampleServerPath"] ?? "MCPSharp.Example.dll";
            public static int TimeoutSeconds => int.TryParse(Configuration["TestSettings:TimeoutSeconds"], out var timeout) ? timeout : 30;
        }
    }
}


