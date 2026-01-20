using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// The content contains video data
    /// </summary>
    public class VideoContent : IContent
    {
        /// <summary>
        /// The type of the content
        /// </summary>
        public string Type { get; set; } = "video";
        /// <summary>
        /// The base64 encoded video data
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// The mime type of the video (e.g., video/mp4, video/webm, video/ogg)
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Default constructor for JSON deserialization
        /// </summary>
        public VideoContent() { }

        /// <summary>
        /// Creates a video content instance
        /// </summary>
        /// <param name="base64String">The base64 encoded video data</param>
        /// <param name="mimeType">The mime type of the video (e.g., video/mp4, video/webm)</param>
        public VideoContent(string base64String, string mimeType)
        {
            Data = base64String;
            MimeType = mimeType;
        }
    }
}

