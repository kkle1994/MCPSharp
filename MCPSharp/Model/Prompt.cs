#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace MCPSharp.Model
{
    /// <summary>
    /// Represents a prompt template in the MCP protocol.
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// The unique identifier for the prompt.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional human-readable title for the prompt.
        /// </summary>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        /// <summary>
        /// Optional description of what the prompt does.
        /// </summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        /// <summary>
        /// Optional list of arguments that can be passed to the prompt.
        /// </summary>
        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PromptArgument>? Arguments { get; set; }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

