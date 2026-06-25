using System.Text.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RmsMcpServer;

/// <summary>
/// Minimal MCP server implementation that communicates via stdio JSON-RPC
/// This is a simplified implementation demonstrating the concept
/// </summary>
public class RmsMcpServerImplementation
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, ToolDefinition> _tools = new();

    public RmsMcpServerImplementation(ILogger<RmsMcpServerImplementation> logger)
    {
        _logger = logger;
        RegisterTools();
        _logger.LogInformation("RMS MCP Server initialized in DEMO mode with {Count} tools", _tools.Count);
    }

    private void RegisterTools()
    {
        RegisterTool("search_global_subjects",
            "Search for persons or businesses in RMS by name, DOB, SSN, or other identifiers",
            SearchGlobalSubjectsAsync);

        RegisterTool("get_person_detail",
            "Get detailed information about a specific person including demographics, addresses, and aliases",
            GetPersonDetailAsync);

        RegisterTool("search_incidents",
            "Search for RMS incidents/reports by date range, case number, or location",
            SearchIncidentsAsync);

        RegisterTool("get_incident_detail",
            "Get complete details for a specific incident including offenses, subjects, and narrative",
            GetIncidentDetailAsync);

        RegisterTool("search_arrests",
            "Search for arrests by date range, person, or agency",
            SearchArrestsAsync);

        RegisterTool("search_warrants",
            "Search for active or historical warrants by person or warrant number",
            SearchWarrantsAsync);

        RegisterTool("search_alerts",
            "Search for alerts/cautions associated with persons, locations, or vehicles",
            SearchAlertsAsync);

        RegisterTool("get_person_activity",
            "Get complete activity history for a person including incidents, arrests, and citations",
            GetPersonActivityAsync);
    }

    private void RegisterTool(string name, string description, Func<JsonElement, Task<string>> handler)
    {
        _tools[name] = new ToolDefinition
        {
            Name = name,
            Description = description,
            Handler = handler
        };
    }

    public async Task<string> ProcessRequestAsync(string json)
    {
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var method = root.GetProperty("method").GetString();
        var id = root.TryGetProperty("id", out var idEl) ? (int?)idEl.GetInt32() : null;
        var paramsEl = root.TryGetProperty("params", out var p) ? p : (JsonElement?)null;

        _logger.LogInformation("Received {Method} request", method);

        return method switch
        {
            "initialize" => await GetInitializeResponse(id),
            "tools/list" => await GetToolsList(id),
            "tools/call" => paramsEl.HasValue ? await HandleToolCall(paramsEl.Value, id) : await GetError(-32602, "Invalid params", id),
            _ => await GetError(-32601, "Method not found", id)
        };
    }

    private Task<string> GetInitializeResponse(int? id)
    {
        var response = new
        {
            jsonrpc = "2.0",
            id,
            result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "rms-mcp-server",
                    version = "1.0.0"
                }
            }
        };

        return Task.FromResult(JsonConvert.SerializeObject(response));
    }

    private Task<string> GetToolsList(int? id)
    {
        var response = new
        {
            jsonrpc = "2.0",
            id,
            result = new
            {
                tools = _tools.Values.Select(t => new
                {
                    name = t.Name,
                    description = t.Description,
                    inputSchema = new
                    {
                        type = "object",
                        properties = new { }
                    }
                })
            }
        };

        return Task.FromResult(JsonConvert.SerializeObject(response));
    }

    private async Task<string> HandleToolCall(JsonElement paramsEl, int? id)
    {
        var toolName = paramsEl.GetProperty("name").GetString();
        var args = paramsEl.TryGetProperty("arguments", out var a) ? a : new JsonElement();

        if (!_tools.TryGetValue(toolName!, out var tool))
        {
            return await GetError(-32602, $"Tool '{toolName}' not found", id);
        }

        try
        {
            var result = await tool.Handler(args);

            var response = new
            {
                jsonrpc = "2.0",
                id,
                result = new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = result
                        }
                    }
                }
            };

            return JsonConvert.SerializeObject(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {Tool}", toolName);
            return await GetError(-32603, $"Tool execution failed: {ex.Message}", id);
        }
    }

    private Task<string> GetError(int code, string message, int? id)
    {
        var response = new
        {
            jsonrpc = "2.0",
            id,
            error = new
            {
                code,
                message
            }
        };

        return Task.FromResult(JsonConvert.SerializeObject(response));
    }

    // Tool handlers - all return demo data
    private Task<string> SearchGlobalSubjectsAsync(JsonElement args) =>
        Task.FromResult(DemoData.GetSearchGlobalSubjectsResponse());

    private Task<string> GetPersonDetailAsync(JsonElement args)
    {
        var personId = args.TryGetProperty("personId", out var p) ? p.GetInt32() : 12345;
        return Task.FromResult(DemoData.GetPersonDetailResponse(personId));
    }

    private Task<string> SearchIncidentsAsync(JsonElement args) =>
        Task.FromResult(DemoData.GetSearchIncidentsResponse());

    private Task<string> GetIncidentDetailAsync(JsonElement args)
    {
        var incidentId = args.TryGetProperty("incidentId", out var i) ? i.GetInt32() : 2024001234;
        return Task.FromResult(DemoData.GetIncidentDetailResponse(incidentId));
    }

    private Task<string> SearchArrestsAsync(JsonElement args) =>
        Task.FromResult(DemoData.GetSearchArrestsResponse());

    private Task<string> SearchWarrantsAsync(JsonElement args) =>
        Task.FromResult(DemoData.GetSearchWarrantsResponse());

    private Task<string> SearchAlertsAsync(JsonElement args) =>
        Task.FromResult(DemoData.GetSearchAlertsResponse());

    private Task<string> GetPersonActivityAsync(JsonElement args)
    {
        var globalSubjectId = args.TryGetProperty("globalSubjectId", out var g) ? g.GetInt32() : 12345;
        return Task.FromResult(DemoData.GetPersonActivityResponse(globalSubjectId));
    }

    private class ToolDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Func<JsonElement, Task<string>> Handler { get; set; } = null!;
    }
}
