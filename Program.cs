using RmsMcpServer;
using Services.WebApi.Clients.Records.Public;

var builder = WebApplication.CreateBuilder(args);

// Register mock RMS client for development
builder.Services.AddSingleton<IPublicRecordsClient, MockPublicRecordsClient>();

// Register MCP server implementation
builder.Services.AddSingleton<RmsMcpServerImplementation>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

var server = app.Services.GetRequiredService<RmsMcpServerImplementation>();

// MCP JSON-RPC endpoint - standard MCP HTTP transport endpoint
app.MapPost("/mcp", async (HttpContext context) =>
{
    try
    {
        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();

        app.Logger.LogInformation("Received MCP message: {Json}", json);

        var response = await server.ProcessRequestAsync(json);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(response);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error processing MCP message");
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32603,\"message\":\"Internal server error\"},\"id\":null}");
    }
});

// Service discovery endpoint
app.MapGet("/", () => Results.Ok(new
{
    name = "rms-mcp-server",
    version = "1.0.0",
    protocol = "MCP 2024-11-05",
    transport = "http",
    endpoints = new
    {
        mcp = "/mcp"
    }
}));

app.Logger.LogInformation("RMS MCP Server starting on HTTP transport...");
app.Logger.LogInformation("MCP JSON-RPC endpoint: /mcp");

app.Run();
