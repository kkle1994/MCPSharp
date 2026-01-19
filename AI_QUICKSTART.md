# MCPSharp AI Quick Reference

> .NET library for building MCP (Model Context Protocol) servers and clients. Create tools that AI assistants can discover and use.

## Installation

```bash
dotnet add package MCPSharp
```

## Minimal Server

```csharp
using MCPSharp;

public class MyTools
{
    [McpTool("greet", "Say hello")]
    public static string Greet([McpParameter(true)] string name) => $"Hello, {name}!";
}

// Start server
await MCPServer.StartAsync("MyServer", "1.0.0");
```

## Minimal Client

```csharp
using MCPSharp;

var client = new MCPClient("MyClient", "1.0", "dotnet", "path/to/server.dll");
var result = await client.CallToolAsync("greet", new() { { "name", "World" } });
```

---

## Content Types

| Type | Constructor | Properties |
|------|-------------|------------|
| `TextContent` | `new TextContent(text)` | `Text`, `Type="text"` |
| `ImageContent` | `new ImageContent(base64, mimeType)` | `Data`, `MimeType`, `Type="image"` |
| `AudioContent` | `new AudioContent(base64, mimeType)` | `Data`, `MimeType`, `Type="audio"` |
| `VideoContent` | `new VideoContent(base64, mimeType)` | `Data`, `MimeType`, `Type="video"` |
| `EmbeddedResource` | `EmbeddedResource.FromText(uri, text, mime)` | `Resource`, `Type="resource"` |
| `EmbeddedResource` | `EmbeddedResource.FromBlob(uri, base64, mime)` | `Resource`, `Type="resource"` |

**Namespace:** `MCPSharp.Model.Content`

---

## Result Types

| Type | Usage |
|------|-------|
| `string` | Auto-wrapped as TextContent |
| `ImageListResult` | `new ImageListResult(imageContents)` |
| `AudioListResult` | `new AudioListResult(audioContents)` |
| `VideoListResult` | `new VideoListResult(videoContents)` |
| `ResourceListResult` | `new ResourceListResult(embeddedResources)` |
| `MixedContentResult` | `MixedContentResult.Builder()...Build()` |
| `CallToolErrorResult` | `new CallToolErrorResult(errorMessage)` |

**Namespace:** `MCPSharp.Model.Results`

---

## Code Patterns

### Simple Text Tool

```csharp
[McpTool("echo", "Echo input")]
public static string Echo([McpParameter(true, "Text to echo")] string input) => input;
```

### Return Image

```csharp
[McpTool("get-image", "Return an image")]
public static ImageListResult GetImage()
{
    var content = new ImageContent(base64Data, "image/png");
    return new ImageListResult(new[] { content });
}
```

### Return Audio

```csharp
[McpTool("get-audio", "Return audio")]
public static AudioListResult GetAudio()
{
    var content = new AudioContent(base64Data, "audio/mpeg");
    return new AudioListResult(new[] { content });
}
```

### Return Video

```csharp
[McpTool("get-video", "Return video")]
public static VideoListResult GetVideo()
{
    var content = new VideoContent(base64Data, "video/mp4");
    return new VideoListResult(new[] { content });
}
```

### Return File Resource

```csharp
[McpTool("get-file", "Return file")]
public static ResourceListResult GetFile()
{
    var resource = EmbeddedResource.FromText("file:///doc.txt", "content", "text/plain");
    return new ResourceListResult(new[] { resource });
}
```

### Mixed Content (Builder)

```csharp
[McpTool("rich-response", "Return mixed content")]
public static MixedContentResult RichResponse()
{
    return MixedContentResult.Builder()
        .AddText("Title")
        .AddImage(imgBase64, "image/png")
        .AddAudio(audioBase64, "audio/wav")
        .AddVideo(videoBase64, "video/mp4")
        .AddTextResource("file:///report.txt", "Report content", "text/plain")
        .AddBlobResource("file:///data.bin", blobBase64, "application/octet-stream")
        .Build();
}
```

### Complex Object Parameter

```csharp
public class UserData { public string Name; public int Age; public string[] Tags; }

[McpTool("process-user", "Process user data")]
public static string ProcessUser([McpParameter(true)] UserData user)
{
    return $"User: {user.Name}, Age: {user.Age}, Tags: {string.Join(",", user.Tags)}";
}
```

### Error Handling

```csharp
[McpTool("safe-divide", "Divide two numbers")]
public static CallToolResult Divide(int a, int b)
{
    if (b == 0) return new CallToolErrorResult("Division by zero");
    return new CallToolResult { Content = new[] { new TextContent((a / b).ToString()) } };
}
```

---

## Client Usage

### Get Tools as AIFunctions

```csharp
var client = new MCPClient("Client", "1.0", "dotnet", "server.dll");
IList<AIFunction> functions = await client.GetFunctionsAsync();
// Use with Microsoft.Extensions.AI ChatOptions.Tools
```

### Call Tool

```csharp
var result = await client.CallToolAsync("tool-name", new Dictionary<string, object>
{
    { "param1", "value1" },
    { "param2", 123 }
});

if (!result.IsError)
{
    foreach (var content in result.Content) { /* process */ }
}
```

### List Resources

```csharp
var resources = await client.GetResourcesAsync();
```

---

## Server Configuration

### Register External Tools

```csharp
MCPServer.Register<ExternalToolClass>();
await MCPServer.StartAsync("Server", "1.0");
```

### Dynamic Tool Registration

```csharp
MCPServer.AddToolHandler(new Tool
{
    Name = "dynamic",
    Description = "Dynamic tool",
    InputSchema = new InputSchema
    {
        Type = "object",
        Required = new[] { "input" },
        Properties = new Dictionary<string, ParameterSchema>
        {
            { "input", new ParameterSchema { Type = "string" } }
        }
    }
}, (string input) => $"Got: {input}");
```

---

## MIME Types Reference

| Category | Common Types |
|----------|--------------|
| Image | `image/png`, `image/jpeg`, `image/gif`, `image/webp`, `image/svg+xml` |
| Audio | `audio/mpeg`, `audio/wav`, `audio/ogg`, `audio/webm`, `audio/aac` |
| Video | `video/mp4`, `video/webm`, `video/ogg`, `video/quicktime` |
| Text | `text/plain`, `text/html`, `text/markdown`, `application/json`, `application/xml` |
| Binary | `application/octet-stream`, `application/pdf`, `application/zip` |

---

## Attributes Quick Reference

```csharp
// Tool definition
[McpTool]                           // Auto-name from method
[McpTool("name")]                   // Custom name
[McpTool("name", "description")]    // Name + description

// Parameter definition
[McpParameter]                      // Optional parameter
[McpParameter(true)]                // Required parameter
[McpParameter(true, "description")] // Required + description
[McpParameter(false, "description")]// Optional + description

// Resource definition
[McpResource("name", "uri://path")]
[McpResource("name", "uri://path", "mime/type", "description")]
```

---

## Imports

```csharp
using MCPSharp;                    // Core: MCPServer, MCPClient, Attributes
using MCPSharp.Model.Content;      // TextContent, ImageContent, AudioContent, VideoContent, EmbeddedResource
using MCPSharp.Model.Results;      // CallToolResult, ImageListResult, AudioListResult, etc.
```


