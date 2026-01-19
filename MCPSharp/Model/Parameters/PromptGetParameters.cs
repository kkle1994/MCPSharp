#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace MCPSharp.Model.Parameters
{
    /// <summary>
    /// Parameters for the prompts/get method.
    /// </summary>
    public class PromptGetParameters
    {
        /// <summary>
        /// The name of the prompt to retrieve.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional arguments to pass to the prompt.
        /// </summary>
        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Arguments { get; set; }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
