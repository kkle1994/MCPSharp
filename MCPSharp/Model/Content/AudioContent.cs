using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// The content contains audio data
    /// </summary>
    public class AudioContent : IContent
    {
        /// <summary>
        /// The type of the content
        /// </summary>
        public string Type { get; set; } = "audio";
        /// <summary>
        /// The base64 encoded audio data
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// The mime type of the audio (e.g., audio/mpeg, audio/wav, audio/ogg)
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Default constructor for JSON deserialization
        /// </summary>
        public AudioContent() { }

        /// <summary>
        /// Creates an audio content instance
        /// </summary>
        /// <param name="base64String">The base64 encoded audio data</param>
        /// <param name="mimeType">The mime type of the audio (e.g., audio/mpeg, audio/wav)</param>
        public AudioContent(string base64String, string mimeType)
        {
            Data = base64String;
            MimeType = mimeType;
        }
    }
}

