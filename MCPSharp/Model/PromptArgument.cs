#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace MCPSharp.Model
{
    /// <summary>
    /// Represents an argument that can be passed to a prompt.
    /// </summary>
    public class PromptArgument
    {
        /// <summary>
        /// The name of the argument.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the argument.
        /// </summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        /// <summary>
        /// Whether this argument is required.
        /// </summary>
        [JsonPropertyName("required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Required { get; set; }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
