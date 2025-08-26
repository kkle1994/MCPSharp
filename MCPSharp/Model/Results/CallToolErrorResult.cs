using System;
using System.Collections.Generic;
using System.Text;

namespace MCPSharp.Model.Results
{
    /// <summary>
    /// The result only contains error information
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    public class CallToolErrorResult : CallToolResult
    {
        public CallToolErrorResult(string errorMessage)
        {
            IsError = true;
            Content = [new Content.TextContent(errorMessage)];
        }
    }
}
