# RMS MCP Server - IPublicRecordsClient Integration

## Current Status: ✅ Mock Client Injected

The server now uses dependency injection with a mock `IPublicRecordsClient` for development.

## What Was Done

### 1. Created MockPublicRecordsClient (MockPublicRecordsClient.cs)
- Implements `IPublicRecordsClient` interface
- All methods throw `NotImplementedException` 
- Acts as a placeholder - actual data comes from `DemoData.cs`

### 2. Updated RmsMcpServerImplementation.cs
- Now accepts `IPublicRecordsClient` via constructor injection
- Added `_useMockData` flag to detect mock vs real client
- Tool handlers check `_useMockData` before calling DemoData
- Ready to swap in real API calls when configured

### 3. Updated Program.cs
- Registered `MockPublicRecordsClient` as `IPublicRecordsClient` singleton
- Server logs "using MOCK data" on startup

## How to Switch to Real RMS API

### Step 1: Configure Authentication

Add to `appsettings.json`:

```json
{
  "NewWorld": {
    "Rms": {
      "Clients": {
        "Public.API": {
          "BaseUrl": "https://your-rms-api-server.com",
          "Version": "1.0",
          "MaxSearchResultLimit": 100
        }
      }
    }
  },
  "RmsApi": {
    "Authentication": {
      "TokenUri": "https://your-auth-server.com/connect/token",
      "RecordsServerClientId": "your-client-id",
      "RecordsServerEncryptedClientSecret": "base64-encrypted-secret",
      "Audience": "rms-public-api",
      "ClientSecretSigningCertificateThumbprint": "certificate-thumbprint"
    }
  }
}
```

### Step 2: Install Required Certificate

The `SystemSecurityTokenProvider` requires an X.509 certificate with private key:
- Install certificate in Windows certificate store
- Use thumbprint in config above
- Certificate is used to decrypt the client secret

### Step 3: Update Program.cs

Replace mock registration with real client setup:

```csharp
// Remove this line:
builder.Services.AddSingleton<IPublicRecordsClient, MockPublicRecordsClient>();

// Add these lines:
builder.Services.AddHttpClient<IPublicRecordsClient, PublicRecordsClient>();

builder.Services.Configure<PublicRecordsClientOptions>(
    builder.Configuration.GetSection("NewWorld:Rms:Clients:Public.API"));

builder.Services.Configure<SecurityTokenProviderOptions>(config =>
{
    var authSection = builder.Configuration.GetSection("RmsApi:Authentication");
    config.TokenUri = authSection["TokenUri"];
    config.RecordsServerClientId = authSection["RecordsServerClientId"];
    config.RecordsServerEncryptedClientSecret = authSection["RecordsServerEncryptedClientSecret"];
    config.Audience = authSection["Audience"];
    config.ClientSecretSigningCertificateThumbprint = authSection["ClientSecretSigningCertificateThumbprint"];
});

builder.Services.AddSingleton<ISigningCertificateProvider, SigningCertificateProvider>();
builder.Services.AddSingleton<ISecurityTokenProvider, SystemSecurityTokenProvider>();
builder.Services.AddSingleton<JwtSecurityTokenHandler>();
```

### Step 4: Implement Real API Calls

In `RmsMcpServerImplementation.cs`, replace the TODO comments with real implementations:

```csharp
private async Task<string> SearchGlobalSubjectsAsync(JsonElement args)
{
    if (_useMockData)
    {
        return Task.FromResult(DemoData.GetSearchGlobalSubjectsResponse());
    }

    // Replace with:
    var searchString = args.TryGetProperty("searchString", out var s) ? s.GetString() : null;
    
    var request = new GlobalSubjectSearchRequest
    {
        // Map args to request properties
        // Note: Check the actual request properties - may differ from simple SearchString
    };
    
    var token = await GetBearerTokenAsync(); // Need to inject ISecurityTokenProvider
    var response = await _rmsClient!.SearchGlobalSubjectsAsync(
        request, 
        token, 
        CancellationToken.None);
    
    return JsonConvert.SerializeObject(response, Formatting.Indented);
}
```

### Step 5: Add Token Provider

Update constructor to inject `ISecurityTokenProvider`:

```csharp
private readonly ISecurityTokenProvider _tokenProvider;

public RmsMcpServerImplementation(
    ILogger<RmsMcpServerImplementation> logger,
    IPublicRecordsClient? rmsClient = null,
    ISecurityTokenProvider? tokenProvider = null)
{
    _logger = logger;
    _rmsClient = rmsClient;
    _tokenProvider = tokenProvider;
    _useMockData = rmsClient == null || rmsClient is MockPublicRecordsClient;
    // ...
}

private async Task<string> GetBearerTokenAsync()
{
    if (_tokenProvider == null) return string.Empty;
    
    var httpClient = new HttpClient(); // Or inject IHttpClientFactory
    var token = await _tokenProvider.GetTokenAsync(httpClient, CancellationToken.None);
    return token.Value;
}
```

## API Mapping Notes

### Search Incidents Issue
The RMS Public API doesn't have a `SearchIncidentsAsync` method. It uses **Cases** instead:
- Use `GetCaseDetailAsync(caseId, ...)` for incident details
- May need to find a case search method or use different approach

### Request Property Mapping
Check the actual request object properties in:
- `NewWorld.Rms.Services.WebApi.Public.Contracts.GlobalSubjects.GlobalSubjectSearchRequest`
- `NewWorld.Rms.Services.WebApi.Public.Contracts.Alerts.AlertSearchRequest`
- `NewWorld.Rms.Services.WebApi.Public.Contracts.Arrests.ArrestSearchRequest`
- `NewWorld.Rms.Services.WebApi.Public.Contracts.Warrants.WarrantSearchRequest`

The properties may not be simple strings like "searchString" - they're likely more complex filter objects.

## Benefits of This Approach

✅ **Clean architecture** - Interface-based dependency injection  
✅ **Easy testing** - Can inject mock or real client  
✅ **Configuration-based** - Switch between mock/real via DI registration  
✅ **Type safety** - Real RMS types when ready  
✅ **Gradual migration** - Can implement one tool at a time

## Current Behavior

Server currently returns demo data from `DemoData.cs` through the mock client pattern. All 8 MCP tools work as before but are now ready for real API integration.
