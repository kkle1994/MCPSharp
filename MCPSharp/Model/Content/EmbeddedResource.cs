using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// The content contains an embedded resource reference
    /// </summary>
    public class EmbeddedResource : IContent
    {
        /// <summary>
        /// The type of the content
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "resource";

        /// <summary>
        /// The resource contents (URI, text/blob data, and MIME type)
        /// </summary>
        [JsonPropertyName("resource")]
        public ResourceContents Resource { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EmbeddedResource() { }

        /// <summary>
        /// Creates an embedded resource with the specified resource contents
        /// </summary>
        /// <param name="resource">The resource contents</param>
        public EmbeddedResource(ResourceContents resource)
        {
            Resource = resource;
        }

        /// <summary>
        /// Creates an embedded resource with a text resource
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="text">The text content</param>
        /// <param name="mimeType">Optional MIME type (defaults to text/plain)</param>
        public static EmbeddedResource FromText(string uri, string text, string mimeType = "text/plain")
        {
            return new EmbeddedResource(ResourceContents.FromText(uri, text, mimeType));
        }

        /// <summary>
        /// Creates an embedded resource with binary content
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="base64Data">The base64 encoded binary data</param>
        /// <param name="mimeType">The MIME type of the binary content</param>
        public static EmbeddedResource FromBlob(string uri, string base64Data, string mimeType)
        {
            return new EmbeddedResource(ResourceContents.FromBlob(uri, base64Data, mimeType));
        }
    }
}


