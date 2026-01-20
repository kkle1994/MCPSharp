#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace MCPSharp.Model
{
    /// <summary>
    /// Represents a message in a prompt response.
    /// </summary>
    public class PromptMessage
    {
        /// <summary>
        /// The role of the message sender ("user" or "assistant").
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public PromptContent Content { get; set; } = new();
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

