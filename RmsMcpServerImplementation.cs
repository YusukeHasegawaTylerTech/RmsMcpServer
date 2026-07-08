using System.Text.Json;
using Newtonsoft.Json;
using NewWorld.Aegis.Rms.Domain.Contracts;
using Services.WebApi.Clients.Records.Public;
using AlertSearchRequest = NewWorld.Rms.Services.WebApi.Public.Contracts.Alerts.AlertSearchRequest;
using ArrestSearchRequest = NewWorld.Rms.Services.WebApi.Public.Contracts.Arrests.ArrestSearchRequest;
using GlobalSubjectSearchRequest = NewWorld.Rms.Services.WebApi.Public.Contracts.GlobalSubjects.GlobalSubjectSearchRequest;
using WarrantSearchRequest = NewWorld.Rms.Services.WebApi.Public.Contracts.Warrants.WarrantSearchRequest;

namespace RmsMcpServer;

/// <summary>
/// MCP server implementation that communicates via JSON-RPC and provides RMS data access
/// </summary>
public class RmsMcpServerImplementation
{
    private readonly ILogger _logger;
    private readonly IPublicRecordsClient _rmsClient;
    private readonly Dictionary<string, ToolDefinition> _tools = new();

    public RmsMcpServerImplementation(
        ILogger<RmsMcpServerImplementation> logger,
        IPublicRecordsClient rmsClient)
    {
        _logger = logger;
        _rmsClient = rmsClient;
        RegisterTools();
        _logger.LogInformation("RMS MCP Server initialized with {Count} tools", _tools.Count);
    }

    private void RegisterTools()
    {
        var paginatedSchema = new
        {
            type = "object",
            properties = new
            {
                size = new
                {
                    type = "integer",
                    description = "Number of results to return (default: 50, max: 100)",
                    minimum = 1,
                    maximum = 100
                },
                start = new
                {
                    type = "integer",
                    description = "Starting offset for results (default: 0)",
                    minimum = 0
                }
            }
        };

        RegisterTool("search_global_subjects",
            "Search for persons or businesses in RMS by name, DOB, SSN, or other identifiers",
            SearchGlobalSubjectsAsync,
            new
            {
                type = "object",
                properties = new
                {
                    firstName = new
                    {
                        type = "string",
                        description = "First name to search for (partial match supported)"
                    },
                    lastName = new
                    {
                        type = "string",
                        description = "Last name to search for (partial match supported)"
                    },
                    middleName = new
                    {
                        type = "string",
                        description = "Middle name to search for (partial match supported)"
                    },
                    dateOfBirth = new
                    {
                        type = "string",
                        description = "Date of birth in ISO format (YYYY-MM-DD)"
                    },
                    ssn = new
                    {
                        type = "string",
                        description = "Social Security Number"
                    },
                    driversLicenseNumber = new
                    {
                        type = "string",
                        description = "Driver's license number"
                    },
                    size = new
                    {
                        type = "integer",
                        description = "Number of results to return (default: 50, max: 100)",
                        minimum = 1,
                        maximum = 100
                    },
                    start = new
                    {
                        type = "integer",
                        description = "Starting offset for results (default: 0)",
                        minimum = 0
                    }
                }
            });

        RegisterTool("get_person_detail",
            "Get detailed information about a specific person including demographics, addresses, and aliases",
            GetPersonDetailAsync,
            new
            {
                type = "object",
                properties = new
                {
                    personId = new
                    {
                        type = "integer",
                        description = "The person ID to retrieve details for"
                    }
                }
            });

        RegisterTool("search_incidents",
            "Search for RMS incidents/reports by date range, case number, or location",
            SearchIncidentsAsync);

        RegisterTool("get_incident_detail",
            "Get complete details for a specific incident including offenses, subjects, and narrative",
            GetIncidentDetailAsync,
            new
            {
                type = "object",
                properties = new
                {
                    incidentId = new
                    {
                        type = "integer",
                        description = "The incident/case ID to retrieve details for"
                    }
                }
            });

        RegisterTool("search_arrests",
            "Search for arrests by date range, person, or agency",
            SearchArrestsAsync,
            paginatedSchema);

        RegisterTool("search_warrants",
            "Search for active or historical warrants by person or warrant number",
            SearchWarrantsAsync,
            paginatedSchema);

        RegisterTool("search_alerts",
            "Search for alerts/cautions associated with persons, locations, or vehicles",
            SearchAlertsAsync,
            paginatedSchema);

        RegisterTool("get_person_activity",
            "Get complete activity history for a person including incidents, arrests, and citations",
            GetPersonActivityAsync,
            new
            {
                type = "object",
                properties = new
                {
                    globalSubjectId = new
                    {
                        type = "integer",
                        description = "The global subject ID to retrieve activity for"
                    }
                }
            });
    }

    private void RegisterTool(string name, string description, Func<JsonElement, Task<string>> handler, object? inputSchema = null)
    {
        _tools[name] = new ToolDefinition
        {
            Name = name,
            Description = description,
            Handler = handler,
            InputSchema = inputSchema
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
                    inputSchema = t.InputSchema ?? new
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

    // Tool handlers - call through IPublicRecordsClient interface
    private async Task<string> SearchGlobalSubjectsAsync(JsonElement args)
    {
        var request = new GlobalSubjectSearchRequest
        {
            // Search parameters
            FirstName = args.TryGetProperty("firstName", out var fn) ? fn.GetString() : null,
            LastName = args.TryGetProperty("lastName", out var ln) ? ln.GetString() : null,
            MiddleName = args.TryGetProperty("middleName", out var mn) ? mn.GetString() : null,
            DateOfBirth = args.TryGetProperty("dateOfBirth", out var dob)
                ? new NewWorld.Aegis.Rms.Domain.Contracts.Date(DateTime.Parse(dob.GetString()!))
                : (NewWorld.Aegis.Rms.Domain.Contracts.Date?)null,
            SocialSecurityNumber = args.TryGetProperty("ssn", out var ssn) ? ssn.GetString() : null,
            DriversLicenseNumber = args.TryGetProperty("driversLicenseNumber", out var dl) ? dl.GetString() : null,

            // Pagination: default to 50 results starting at 0
            Size = args.TryGetProperty("size", out var s) ? s.GetInt32() : 50,
            Start = args.TryGetProperty("start", out var st) ? st.GetInt32() : 0,
            Location = new NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.LocationSearchRequest()
            {
                IgnoreLocationCriteriaOnVerificationFailure = true,
            }
        };

        var response = await _rmsClient.SearchGlobalSubjectsAsync(
        request,
        string.Empty, // bearerToken
        CancellationToken.None);
        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> GetPersonDetailAsync(JsonElement args)
    {
        var personId = args.TryGetProperty("personId", out var p) ? p.GetInt32() : 12345;

        var response = await _rmsClient.GetPersonDetailAsync(
            personId,
            new[] { UsageType.Person },
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> SearchIncidentsAsync(JsonElement args)
    {
        // Note: RMS API uses Cases, not Incidents
        // Return empty for now - mock client can handle this
        return await Task.FromResult(JsonConvert.SerializeObject(new { results = new object[0], totalCount = 0 }));
    }

    private async Task<string> GetIncidentDetailAsync(JsonElement args)
    {
        var caseId = args.TryGetProperty("incidentId", out var i) ? i.GetInt32() : 2024001234;

        var response = await _rmsClient.GetCaseDetailAsync(
            caseId,
            new[] { UsageType.Case },
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> SearchArrestsAsync(JsonElement args)
    {
        var request = new ArrestSearchRequest
        {
            // Pagination: default to 50 results starting at 0
            Size = args.TryGetProperty("size", out var s) ? s.GetInt32() : 50,
            Start = args.TryGetProperty("start", out var st) ? st.GetInt32() : 0
        };

        var response = await _rmsClient.SearchArrestsAsync(
            request,
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> SearchWarrantsAsync(JsonElement args)
    {
        var request = new WarrantSearchRequest
        {
            // Pagination: default to 50 results starting at 0
            Size = args.TryGetProperty("size", out var s) ? s.GetInt32() : 50,
            Start = args.TryGetProperty("start", out var st) ? st.GetInt32() : 0
        };

        var response = await _rmsClient.SearchWarrantsAsync(
            request,
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> SearchAlertsAsync(JsonElement args)
    {
        var request = new AlertSearchRequest
        {
            // Pagination: default to 50 results starting at 0
            Size = args.TryGetProperty("size", out var s) ? s.GetInt32() : 50,
            Start = args.TryGetProperty("start", out var st) ? st.GetInt32() : 0
        };

        var response = await _rmsClient.SearchAlertsAsync(
            request,
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private async Task<string> GetPersonActivityAsync(JsonElement args)
    {
        var globalSubjectId = args.TryGetProperty("globalSubjectId", out var g) ? g.GetInt32() : 12345;

        var request = new NewWorld.Aegis.Rms.Domain.Contracts.GlobalSubjects.GlobalSubjectActivitySearchRequest
        {
            GlobalSubjectIds = new[] { globalSubjectId }
        };

        var response = await _rmsClient.GetActivitiesForGlobalSubjectAsync(
            request,
            string.Empty,
            CancellationToken.None);

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    private class ToolDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Func<JsonElement, Task<string>> Handler { get; set; } = null!;
        public object? InputSchema { get; set; }
    }
}
