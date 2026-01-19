using MCPSharp.Model;
using MCPSharp.Model.Results;
using System.Reflection;
using System.Text.Json;

namespace MCPSharp.Core.Prompts
{
    /// <summary>
    /// Handles the execution of a registered prompt.
    /// </summary>
    public class PromptHandler(Prompt prompt, MethodInfo method)
    {
        /// <summary>
        /// The prompt metadata.
        /// </summary>
        public Prompt Prompt = prompt;
        
        private readonly MethodInfo _method = method;

        /// <summary>
        /// Executes the prompt with the given arguments and returns the result.
        /// </summary>
        /// <param name="arguments">Arguments to pass to the prompt.</param>
        /// <returns>The prompt result containing messages.</returns>
        public async Task<PromptGetResult> HandleAsync(Dictionary<string, object>? arguments)
        {
            try
            {
                var inputValues = new Dictionary<string, object>();
                foreach (var par in _method.GetParameters())
                {
                    if (arguments != null && arguments.TryGetValue(par.Name!, out var value))
                    {
                        if (value is JsonElement element)
                        {
                            value = JsonSerializer.Deserialize(element.GetRawText(), par.ParameterType);
                        }
                        inputValues.Add(par.Name!, value!);
                    }
                    else if (par.HasDefaultValue)
                    {
                        inputValues.Add(par.Name!, par.DefaultValue!);
                    }
                    else
                    {
                        inputValues.Add(par.Name!, par.ParameterType.IsValueType ? Activator.CreateInstance(par.ParameterType)! : null!);
                    }
                }

                var result = _method.Invoke(Activator.CreateInstance(_method.DeclaringType), [.. inputValues.Values]);

                if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }

                if (result is PromptGetResult promptResult)
                    return promptResult;

                // If the method returns a string, wrap it in a PromptGetResult
                if (result is string resultString)
                {
                    return new PromptGetResult
                    {
                        Messages = new List<PromptMessage>
                        {
                            new PromptMessage
                            {
                                Role = "user",
                                Content = new PromptContent
                                {
                                    Type = "text",
                                    Text = resultString
                                }
                            }
                        }
                    };
                }

                // Default empty result
                return new PromptGetResult
                {
                    Messages = new List<PromptMessage>()
                };
            }
            catch (Exception ex)
            {
                var e = ex is TargetInvocationException tie ? tie.InnerException ?? tie : ex;
                
                return new PromptGetResult
                {
                    Description = $"Error: {e.Message}",
                    Messages = new List<PromptMessage>
                    {
                        new PromptMessage
                        {
                            Role = "user",
                            Content = new PromptContent
                            {
                                Type = "text",
                                Text = $"Error executing prompt: {e.Message}"
                            }
                        }
                    }
                };
            }
        }
    }
}
