using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NewWorld.Aegis.Rms.Domain.Contracts;
using NewWorld.Aegis.Rms.Domain.Contracts.Agencies;
using NewWorld.Aegis.Rms.Domain.Contracts.Alerts;
using NewWorld.Aegis.Rms.Domain.Contracts.Arrests;
using NewWorld.Aegis.Rms.Domain.Contracts.Cases;
using NewWorld.Aegis.Rms.Domain.Contracts.Documents;
using NewWorld.Aegis.Rms.Domain.Contracts.Extensions;
using NewWorld.Aegis.Rms.Domain.Contracts.GlobalSubjects;
using NewWorld.Aegis.Rms.Domain.Contracts.GlobalSubjects.Persons;
using NewWorld.Aegis.Rms.Domain.Contracts.Locations;
using NewWorld.Aegis.Rms.Domain.Contracts.Personnels;
using NewWorld.Aegis.Rms.Domain.Contracts.Search;
using NewWorld.Aegis.Rms.Domain.Contracts.Statutes;
using NewWorld.Aegis.Rms.Domain.Contracts.Synchronization;
using NewWorld.Aegis.Rms.Domain.Contracts.ValidationSets;
using NewWorld.Aegis.Rms.Domain.Contracts.Warrants;
using NewWorld.Rms.Services.WebApi.Public.Contracts.Domain;
using Services.WebApi.Clients.Records.Public;

namespace RmsMcpServer;

/// <summary>
/// Mock implementation of IPublicRecordsClient that returns demo data
/// This allows the MCP server to work through the interface without a live RMS connection
/// </summary>
public class MockPublicRecordsClient : IPublicRecordsClient
{
    public Task<NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.BatchMatchResult<NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.MatchRequest<string>, MatchResult<int, Agency>>> MatchAgenciesAsync(
        BatchMatchRequest<NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.MatchRequest<string>> matchRequests,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Mock: MatchAgenciesAsync not used by MCP tools");
    }

    public Task<NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.BatchMatchResult<ValidationSetEntryMatch, ValidationSetEntryMatch>> MatchValidationSetEntriesAsync(
        BatchMatchRequest<ValidationSetEntryMatch> validationSetEntryMatchRequests,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Mock: MatchValidationSetEntriesAsync not used by MCP tools");
    }

    public Task<NewWorld.Rms.Services.WebApi.Public.Contracts.Domain.BatchMatchResult<PersonScarMarkTattooIdentifyingFeatureMatch, PersonScarMarkTattooIdentifyingFeatureMatch>> MatchPersonScarMarkTattooIdentifyingFeatureAsync(
        BatchMatchRequest<PersonScarMarkTattooIdentifyingFeatureMatch> personScarMarkTattooIdentifyingFeatureMatchRequests,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Mock: MatchPersonScarMarkTattooIdentifyingFeatureAsync not used by MCP tools");
    }

    public Task<SearchResponse<NewWorld.Rms.Services.WebApi.Public.Contracts.GlobalSubjects.GlobalSubjectSearchResult>> SearchGlobalSubjectsAsync(
        NewWorld.Rms.Services.WebApi.Public.Contracts.GlobalSubjects.GlobalSubjectSearchRequest globalSubjectSearchRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        // Deserialize the demo data JSON into the proper response type
        var json = DemoData.GetSearchGlobalSubjectsResponse();
        var response = JsonConvert.DeserializeObject<SearchResponse<NewWorld.Rms.Services.WebApi.Public.Contracts.GlobalSubjects.GlobalSubjectSearchResult>>(json);
        return Task.FromResult(response!);
    }

    public Task<SearchResponse<AlertSearchResponse>> SearchAlertsAsync(
        NewWorld.Rms.Services.WebApi.Public.Contracts.Alerts.AlertSearchRequest alertSearchRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var json = DemoData.GetSearchAlertsResponse();
        var response = JsonConvert.DeserializeObject<SearchResponse<AlertSearchResponse>>(json);
        return Task.FromResult(response!);
    }

    public Task<PersonDetail> GetPersonDetailAsync(
        int personId,
        IEnumerable<UsageType> include,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var json = DemoData.GetPersonDetailResponse(personId);
        var response = JsonConvert.DeserializeObject<PersonDetail>(json);
        return Task.FromResult(response!);
    }

    public Task<SearchResponse<GlobalSubjectActivitySearchResponse>> GetActivitiesForGlobalSubjectAsync(
        GlobalSubjectActivitySearchRequest globalSubjectActivitySearchRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var globalSubjectId = globalSubjectActivitySearchRequest.GlobalSubjectIds?.FirstOrDefault() ?? 12345;
        var json = DemoData.GetPersonActivityResponse(globalSubjectId);
        var response = JsonConvert.DeserializeObject<SearchResponse<GlobalSubjectActivitySearchResponse>>(json);
        return Task.FromResult(response!);
    }

    public Task<SearchResponse<WarrantSearchResult>> SearchWarrantsAsync(
        NewWorld.Rms.Services.WebApi.Public.Contracts.Warrants.WarrantSearchRequest warrantSearchRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var json = DemoData.GetSearchWarrantsResponse();
        var response = JsonConvert.DeserializeObject<SearchResponse<WarrantSearchResult>>(json);
        return Task.FromResult(response!);
    }

    public Task<SearchResponse<ArrestSearchResult>> SearchArrestsAsync(
        NewWorld.Rms.Services.WebApi.Public.Contracts.Arrests.ArrestSearchRequest arrestSearchRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var json = DemoData.GetSearchArrestsResponse();
        var response = JsonConvert.DeserializeObject<SearchResponse<ArrestSearchResult>>(json);
        return Task.FromResult(response!);
    }

    public Task<WarrantDetail> GetWarrantDetailAsync(
        int warrantId,
        IEnumerable<UsageType> include,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        // DemoData doesn't have a warrant detail method, create minimal response
        var warrant = new WarrantDetail { Id = warrantId };
        return Task.FromResult(warrant);
    }

    public Task<ArrestDetail> GetArrestDetailAsync(
        int arrestId,
        IEnumerable<UsageType> include,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        // DemoData doesn't have an arrest detail method, create minimal response
        var arrest = new ArrestDetail { Id = arrestId };
        return Task.FromResult(arrest);
    }

    public Task<CaseDetail> GetCaseDetailAsync(
        int caseId,
        IEnumerable<UsageType> include,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        var json = DemoData.GetIncidentDetailResponse(caseId);
        var response = JsonConvert.DeserializeObject<CaseDetail>(json);
        return Task.FromResult(response!);
    }

    // Methods not used by MCP tools - return empty/default values
    public Task<List<PropertyValidationSetDetail>> GetPropertyValidationSetDetailsAsync(
        IEnumerable<int> propertyTypeIds,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<PropertyValidationSetDetail>());
    }

    public Task<List<ExtensionAlternateValue>> GetExtensionValuesAsync(
        int extensionId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<ExtensionAlternateValue>());
    }

    public Task<List<Agency>> GetAgenciesAsync(
        IEnumerable<int> agencyIds,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Agency>());
    }

    public Task<Location> GetLocation(int value, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Location());
    }

    public Task<List<DocumentMetadata>> GetDocumentMetadataAsync(
        int recordId,
        UsageType usageType,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<DocumentMetadata>());
    }

    public Task<List<GlobalSubjectPhoto>> GetGlobalSubjectPhotosAsync(
        int recordId,
        SubModuleType subModuleType,
        long subRecordId,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<GlobalSubjectPhoto>());
    }

    public (string url, IEnumerable<KeyValuePair<string, string>> httpHeaders) GetDocumentDownloadRoute(
        string documentId,
        bool IsPersonPhoto,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return ("http://mock/document", new List<KeyValuePair<string, string>>());
    }

    public Task<List<GlobalSubjectContactInformationDetail>> GetGlobalSubjectContactInformationsAsync(
        IEnumerable<int> globalSubjectId,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<GlobalSubjectContactInformationDetail>());
    }

    public Task<SynchronizationResponse<CrimeCode>> SynchronizeCrimeCodesAsync(
        SynchronizationRequest synchronizationRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new SynchronizationResponse<CrimeCode>());
    }

    public Task<SynchronizationResponse<Statute>> SynchronizeStatutesAsync(
        SynchronizationRequest synchronizationRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new SynchronizationResponse<Statute>());
    }

    public Task<SynchronizationResponse<SynchronizationPersonnel>> SynchronizePersonnelAsync(
        SynchronizationRequest synchronizationRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new SynchronizationResponse<SynchronizationPersonnel>());
    }

    public Task<WarrantUpdateStatusResponse> UpdateWarrantStatusAsync(
        WarrantUpdateStatusRequest warrantUpdateStatusRequest,
        string bearerToken,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new WarrantUpdateStatusResponse());
    }
}
