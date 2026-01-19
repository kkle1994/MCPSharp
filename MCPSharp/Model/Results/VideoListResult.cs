using MCPSharp.Model.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result contains a list of video content
    /// </summary>
    public class VideoListResult : CallToolResult
    {
        /// <summary>
        /// Creates a result with video content list
        /// </summary>
        /// <param name="videoContents">The list of video content</param>
        public VideoListResult(IEnumerable<VideoContent> videoContents)
        {
            Content = videoContents;
        }
    }
}


