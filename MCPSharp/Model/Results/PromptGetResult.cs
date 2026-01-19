#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// Result of the prompts/get method.
    /// </summary>
    public class PromptGetResult
    {
        /// <summary>
        /// Optional description of the prompt instance.
        /// </summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        /// <summary>
        /// The messages that make up the prompt.
        /// </summary>
        [JsonPropertyName("messages")]
        public List<PromptMessage> Messages { get; set; } = new();
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
