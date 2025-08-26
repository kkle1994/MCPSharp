using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Content
{
    /// <summary>
    /// The interface for all content types
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// The type of the content
        /// </summary>
        public string Type { get; set; }
    }
}
