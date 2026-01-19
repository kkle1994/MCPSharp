#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using MCPSharp.Model.Content;
using System.Text.Json.Serialization;

namespace MCPSharp.Model
{
    /// <summary>
    /// Represents the content of a prompt message.
    /// </summary>
    public class PromptContent
    {
        /// <summary>
        /// The type of content ("text", "image", "audio", "resource").
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        /// <summary>
        /// Text content (when type is "text").
        /// </summary>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        /// <summary>
        /// Base64 encoded data (when type is "image", "audio", etc.).
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Data { get; set; }

        /// <summary>
        /// MIME type of the content.
        /// </summary>
        [JsonPropertyName("mimeType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MimeType { get; set; }

        /// <summary>
        /// Resource contents (when type is "resource").
        /// </summary>
        [JsonPropertyName("resource")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResourceContents? Resource { get; set; }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
