#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MCPSharp
{
    /// <summary>
    /// Attribute to mark a method as an MCP prompt.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class McpPromptAttribute(string name = null, string title = null, string description = null) : Attribute
    {
        /// <summary>
        /// The unique identifier for the prompt.
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Optional human-readable title for the prompt.
        /// </summary>
        public string Title { get; set; } = title;

        /// <summary>
        /// Optional description of what the prompt does.
        /// </summary>
        public string Description { get; set; } = description;
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

