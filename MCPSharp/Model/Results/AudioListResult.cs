using MCPSharp.Model.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result contains a list of audio content
    /// </summary>
    public class AudioListResult : CallToolResult
    {
        /// <summary>
        /// Creates a result with audio content list
        /// </summary>
        /// <param name="audioContents">The list of audio content</param>
        public AudioListResult(IEnumerable<AudioContent> audioContents)
        {
            Content = audioContents;
        }
    }
}


