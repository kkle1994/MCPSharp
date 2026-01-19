using MCPSharp.Model.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result contains a list of embedded resources
    /// </summary>
    public class ResourceListResult : CallToolResult
    {
        /// <summary>
        /// Creates a result with embedded resource list
        /// </summary>
        /// <param name="resourceContents">The list of embedded resources</param>
        public ResourceListResult(IEnumerable<EmbeddedResource> resourceContents)
        {
            Content = resourceContents;
        }
    }
}


