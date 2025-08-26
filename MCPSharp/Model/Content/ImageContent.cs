using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// The content contains an image
    /// </summary>
    public class ImageContent : IContent
    {
        /// <summary>
        /// The type of the content
        /// </summary>
        public string Type { get; set; } = "image";
        /// <summary>
        /// The data of the content
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// The mime type of the content
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The image content
        /// </summary>
        /// <param name="base64String">The base64 string of the image</param>
        /// <param name="mimeType">The mime type of the image</param>
        public ImageContent(string base64String, string mimeType)
        {
            Data = base64String;
            MimeType = mimeType;
        }
    }
}
