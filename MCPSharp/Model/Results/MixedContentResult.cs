using MCPSharp.Model.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result contains a mix of different content types (text, image, audio, video, resource)
    /// </summary>
    public class MixedContentResult : CallToolResult
    {
        /// <summary>
        /// Creates a result with mixed content from an enumerable
        /// </summary>
        /// <param name="contents">The list of mixed content items</param>
        public MixedContentResult(IEnumerable<IContent> contents)
        {
            Content = contents;
        }

        /// <summary>
        /// Creates a result with mixed content from params
        /// </summary>
        /// <param name="contents">The content items</param>
        public MixedContentResult(params IContent[] contents)
        {
            Content = contents;
        }

        /// <summary>
        /// Creates an empty mixed content result
        /// </summary>
        public MixedContentResult()
        {
            Content = Array.Empty<IContent>();
        }

        /// <summary>
        /// Creates a builder for constructing mixed content results
        /// </summary>
        /// <returns>A new MixedContentBuilder instance</returns>
        public static MixedContentBuilder Builder() => new MixedContentBuilder();
    }

    /// <summary>
    /// Builder class for constructing MixedContentResult with fluent API
    /// </summary>
    public class MixedContentBuilder
    {
        private readonly List<IContent> _contents = new List<IContent>();

        /// <summary>
        /// Adds text content
        /// </summary>
        /// <param name="text">The text content</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddText(string text)
        {
            _contents.Add(new TextContent(text));
            return this;
        }

        /// <summary>
        /// Adds image content
        /// </summary>
        /// <param name="base64Data">The base64 encoded image data</param>
        /// <param name="mimeType">The MIME type of the image</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddImage(string base64Data, string mimeType)
        {
            _contents.Add(new ImageContent(base64Data, mimeType));
            return this;
        }

        /// <summary>
        /// Adds audio content
        /// </summary>
        /// <param name="base64Data">The base64 encoded audio data</param>
        /// <param name="mimeType">The MIME type of the audio</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddAudio(string base64Data, string mimeType)
        {
            _contents.Add(new AudioContent(base64Data, mimeType));
            return this;
        }

        /// <summary>
        /// Adds video content
        /// </summary>
        /// <param name="base64Data">The base64 encoded video data</param>
        /// <param name="mimeType">The MIME type of the video</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddVideo(string base64Data, string mimeType)
        {
            _contents.Add(new VideoContent(base64Data, mimeType));
            return this;
        }

        /// <summary>
        /// Adds an embedded resource
        /// </summary>
        /// <param name="resource">The embedded resource</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddResource(EmbeddedResource resource)
        {
            _contents.Add(resource);
            return this;
        }

        /// <summary>
        /// Adds an embedded resource from text
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="text">The text content</param>
        /// <param name="mimeType">Optional MIME type</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddTextResource(string uri, string text, string mimeType = "text/plain")
        {
            _contents.Add(EmbeddedResource.FromText(uri, text, mimeType));
            return this;
        }

        /// <summary>
        /// Adds an embedded resource from binary data
        /// </summary>
        /// <param name="uri">The URI of the resource</param>
        /// <param name="base64Data">The base64 encoded binary data</param>
        /// <param name="mimeType">The MIME type</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder AddBlobResource(string uri, string base64Data, string mimeType)
        {
            _contents.Add(EmbeddedResource.FromBlob(uri, base64Data, mimeType));
            return this;
        }

        /// <summary>
        /// Adds any content item
        /// </summary>
        /// <param name="content">The content item</param>
        /// <returns>The builder instance</returns>
        public MixedContentBuilder Add(IContent content)
        {
            _contents.Add(content);
            return this;
        }

        /// <summary>
        /// Builds the MixedContentResult
        /// </summary>
        /// <returns>The constructed MixedContentResult</returns>
        public MixedContentResult Build()
        {
            return new MixedContentResult(_contents);
        }
    }
}


