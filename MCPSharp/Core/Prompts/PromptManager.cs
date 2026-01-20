using MCPSharp.Model;
using MCPSharp.Model.Schemas;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MCPSharp.Core.Prompts
{
    /// <summary>
    /// Manages prompt registration and retrieval.
    /// </summary>
    public class PromptManager
    {
        /// <summary>
        /// Dictionary of registered prompt handlers keyed by prompt name.
        /// </summary>
        public readonly Dictionary<string, PromptHandler> Prompts = new();

        /// <summary>
        /// Action to call when prompts change.
        /// </summary>
        public Action PromptChangeNotification = () => { };

        /// <summary>
        /// Scans a class for prompts and registers them.
        /// </summary>
        /// <typeparam name="T">The type to scan for prompt methods.</typeparam>
        public void Register<T>() where T : class, new()
        {
            var type = typeof(T);

            foreach (var method in type.GetMethods())
            {
                RegisterPrompt(method);
            }

            PromptChangeNotification.Invoke();
        }

        /// <summary>
        /// Adds a prompt handler directly.
        /// </summary>
        /// <param name="handler">The prompt handler to add.</param>
        public void AddPromptHandler(PromptHandler handler)
        {
            Prompts[handler.Prompt.Name] = handler;
            PromptChangeNotification.Invoke();
        }

        private void RegisterPrompt(MethodInfo method)
        {
            var promptAttr = method.GetCustomAttribute<McpPromptAttribute>();
            if (promptAttr == null) return;

            string name = promptAttr.Name ?? method.Name;
            string? title = promptAttr.Title;
            string? description = promptAttr.Description ?? method.GetXmlDocumentation();

            // Build argument list from method parameters
            var arguments = new List<PromptArgument>();
            foreach (var parameter in method.GetParameters())
            {
                var paramDescription = parameter.GetXmlDocumentation() 
                    ?? parameter.GetCustomAttribute<DescriptionAttribute>()?.Description;
                
                var isRequired = parameter.GetCustomAttribute<RequiredAttribute>() != null 
                    || parameter.GetCustomAttribute<McpParameterAttribute>()?.Required == true
                    || !parameter.HasDefaultValue;

                arguments.Add(new PromptArgument
                {
                    Name = parameter.Name!,
                    Description = paramDescription,
                    Required = isRequired
                });
            }

            var prompt = new Prompt
            {
                Name = name,
                Title = title,
                Description = description,
                Arguments = arguments.Count > 0 ? arguments : null
            };

            Prompts[name] = new PromptHandler(prompt, method);
        }

        /// <summary>
        /// Gets all registered prompts.
        /// </summary>
        /// <returns>List of all registered prompts.</returns>
        public List<Prompt> GetAllPrompts()
        {
            return Prompts.Values.Select(h => h.Prompt).ToList();
        }

        /// <summary>
        /// Gets a specific prompt by name.
        /// </summary>
        /// <param name="name">The name of the prompt.</param>
        /// <returns>The prompt if found, null otherwise.</returns>
        public Prompt? GetPrompt(string name)
        {
            return Prompts.TryGetValue(name, out var handler) ? handler.Prompt : null;
        }

        /// <summary>
        /// Gets the handler for a specific prompt.
        /// </summary>
        /// <param name="name">The name of the prompt.</param>
        /// <returns>The prompt handler if found, null otherwise.</returns>
        public PromptHandler? GetHandler(string name)
        {
            return Prompts.TryGetValue(name, out var handler) ? handler : null;
        }
    }
}

