using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// Represents the contents of an embedded resource
    /// </summary>
    public class ResourceContents
    {
        /// <summary>
        /// The URI of the resource
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The MIME type of the resource content
        /// </summary>
        [JsonPropertyName("mimeType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MimeType { get; set; }

        /// <summary>
        /// Text content of the resource (for text-based resources)
        /// </summary>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Text { get; set; }

        /// <summary>
        /// Base64 encoded binary content (for binary resources like images, audio, video)
        /// </summary>
        [JsonPropertyName("blob")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Blob { get; set; }

        /// <summary>
        /// Creates a resource contents with text content
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="text">The text content</param>
        /// <param name="mimeType">Optional MIME type</param>
        public static ResourceContents FromText(string uri, string text, string mimeType = "text/plain")
        {
            return new ResourceContents
            {
                Uri = uri,
                Text = text,
                MimeType = mimeType
            };
        }

        /// <summary>
        /// Creates a resource contents with binary (blob) content
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="base64Data">The base64 encoded binary data</param>
        /// <param name="mimeType">The MIME type of the binary content</param>
        public static ResourceContents FromBlob(string uri, string base64Data, string mimeType)
        {
            return new ResourceContents
            {
                Uri = uri,
                Blob = base64Data,
                MimeType = mimeType
            };
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResourceContents() { }

        /// <summary>
        /// Creates a resource contents with URI
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        public ResourceContents(string uri)
        {
            Uri = uri;
        }
    }
}


