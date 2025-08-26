using MCPSharp.Model.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result contains a list of images
    /// </summary>
    public class ImageListResult : CallToolResult
    {
        /// <summary>
        /// The list of images
        /// </summary>
        /// <param name="imageContents">The list of images</param>
        public ImageListResult(IEnumerable<ImageContent> imageContents)
        {
            Content = imageContents;
        }
    }
}
