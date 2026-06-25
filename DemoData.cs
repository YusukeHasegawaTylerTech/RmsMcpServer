using Newtonsoft.Json;

namespace RmsMcpServer;

/// <summary>
/// Provides demo/mock data for testing the MCP server without a live RMS connection
/// </summary>
public static class DemoData
{
    public static string GetSearchGlobalSubjectsResponse()
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    id = 12345,
                    globalSubjectId = 12345,
                    lastName = "SMITH",
                    firstName = "JOHN",
                    middleName = "DAVID",
                    dateOfBirth = "1985-03-15",
                    ssn = "***-**-4567",
                    race = "White",
                    sex = "Male",
                    city = "METROPOLIS",
                    state = "CA",
                    hasWarrants = true,
                    hasAlerts = true
                },
                new
                {
                    id = 12346,
                    globalSubjectId = 12346,
                    lastName = "SMITH",
                    firstName = "JANE",
                    middleName = "MARIE",
                    dateOfBirth = "1990-07-22",
                    ssn = "***-**-7890",
                    race = "White",
                    sex = "Female",
                    city = "SPRINGFIELD",
                    state = "CA",
                    hasWarrants = false,
                    hasAlerts = false
                }
            },
            totalCount = 2,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetPersonDetailResponse(int personId)
    {
        var response = new
        {
            id = personId,
            globalSubjectId = personId,
            general = new
            {
                lastName = "SMITH",
                firstName = "JOHN",
                middleName = "DAVID",
                suffix = null as string,
                dateOfBirth = "1985-03-15",
                age = 41,
                ssn = "123-45-4567",
                driversLicenseNumber = "D1234567",
                driversLicenseState = "CA",
                race = "White",
                ethnicity = "Not Hispanic",
                sex = "Male",
                maritalStatus = "Married",
                citizenship = "US Citizen"
            },
            physicalCharacteristics = new
            {
                height = 72, // inches
                weight = 180, // pounds
                hairColor = "Brown",
                eyeColor = "Blue",
                buildType = "Medium",
                complexion = "Fair",
                glasses = false,
                facialHair = "None"
            },
            addresses = new object[]
            {
                new
                {
                    id = 1001,
                    type = "Home",
                    street = "123 MAIN ST",
                    city = "METROPOLIS",
                    state = "CA",
                    zip = "90001",
                    isPrimary = true,
                    startDate = "2020-01-01",
                    endDate = (string?)null
                },
                new
                {
                    id = 1002,
                    type = "Previous",
                    street = "456 OAK AVE",
                    city = "SPRINGFIELD",
                    state = "CA",
                    zip = "90002",
                    isPrimary = false,
                    startDate = "2015-06-01",
                    endDate = (string?)"2019-12-31"
                }
            },
            aliases = new[]
            {
                new
                {
                    id = 501,
                    lastName = "SMYTHE",
                    firstName = "JOHNNY",
                    middleName = (string?)null,
                    type = "AKA"
                }
            },
            contactInformation = new
            {
                homePhone = "(555) 123-4567",
                cellPhone = "(555) 987-6543",
                workPhone = null as string,
                email = "john.smith@example.com"
            },
            warnings = new string[]
            {
                "CAUTION: Armed and dangerous",
                "Known gang affiliation"
            }
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetSearchIncidentsResponse()
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    id = 2024001234,
                    caseNumber = "24-001234",
                    reportedDate = "2024-06-24T14:30:00Z",
                    occurredStart = "2024-06-24T13:00:00Z",
                    occurredEnd = "2024-06-24T13:30:00Z",
                    summary = "Theft from vehicle - parking lot at 100 Commerce St",
                    primaryOffense = "Theft",
                    location = "100 COMMERCE ST, METROPOLIS, CA",
                    reportingOfficer = "Officer J. Rodriguez (Badge 2547)",
                    status = "Under Investigation",
                    disposition = null as string
                },
                new
                {
                    id = 2024001235,
                    caseNumber = "24-001235",
                    reportedDate = "2024-06-24T18:45:00Z",
                    occurredStart = "2024-06-24T18:00:00Z",
                    occurredEnd = "2024-06-24T18:15:00Z",
                    summary = "Domestic disturbance - verbal argument",
                    primaryOffense = "Disturbance",
                    location = "555 ELM ST APT 4B, METROPOLIS, CA",
                    reportingOfficer = "Officer M. Chen (Badge 1823)",
                    status = "Report Completed",
                    disposition = "No charges filed"
                }
            },
            totalCount = 2,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetIncidentDetailResponse(int incidentId)
    {
        var response = new
        {
            id = incidentId,
            caseNumber = "24-001234",
            reportedDate = "2024-06-24T14:30:00Z",
            occurredStart = "2024-06-24T13:00:00Z",
            occurredEnd = "2024-06-24T13:30:00Z",
            general = new
            {
                summary = "Theft from vehicle - parking lot at 100 Commerce St",
                status = "Under Investigation",
                disposition = null as string,
                reportingOfficer = "Officer J. Rodriguez (Badge 2547)",
                agency = "Metropolis Police Department"
            },
            location = new
            {
                street = "100 COMMERCE ST",
                city = "METROPOLIS",
                state = "CA",
                zip = "90001",
                premiseType = "Parking Lot"
            },
            offenses = new[]
            {
                new
                {
                    id = 1,
                    offense = "Theft From Vehicle",
                    crimeCode = "487(d)(1) PC",
                    attempts = false,
                    completed = true,
                    classification = "Felony"
                }
            },
            subjects = new[]
            {
                new
                {
                    id = 1,
                    globalSubjectId = 54321,
                    name = "DOE, JANE MARIE",
                    role = "Victim",
                    dateOfBirth = "1992-05-10",
                    address = "789 PINE ST, METROPOLIS, CA 90001"
                }
            },
            property = new[]
            {
                new
                {
                    id = 1,
                    description = "Laptop computer - Dell XPS 15",
                    value = 1500.00,
                    status = "Stolen",
                    serialNumber = "ABC123XYZ"
                },
                new
                {
                    id = 2,
                    description = "Backpack - black nylon",
                    value = 50.00,
                    status = "Stolen",
                    serialNumber = null as string
                }
            },
            narrative = "On 06/24/2024 at approximately 1430 hours, I responded to 100 Commerce St regarding a theft from vehicle report. Upon arrival, I contacted the victim, Jane Doe, who stated that she parked her vehicle (2020 Honda Civic, license plate 7ABC123) in the parking lot at approximately 1300 hours. When she returned at approximately 1330 hours, she discovered the passenger window had been smashed and her laptop computer and backpack were missing from the front seat. The laptop is described as a Dell XPS 15 with serial number ABC123XYZ, valued at $1500. The backpack is described as a black nylon backpack valued at $50. There are no witnesses at this time. Crime scene technicians were notified and responded to process the scene for evidence. Case remains under investigation."
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetSearchArrestsResponse()
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    id = 50123,
                    arrestNumber = "A-2024-0567",
                    arrestDate = "2024-06-20T22:15:00Z",
                    globalSubjectId = 12345,
                    subjectName = "SMITH, JOHN DAVID",
                    charges = new[] { "Possession of Controlled Substance", "Paraphernalia" },
                    arrestingOfficer = "Officer K. Williams (Badge 3421)",
                    agency = "Metropolis Police Department",
                    location = "500 MARKET ST, METROPOLIS, CA",
                    status = "Booked"
                }
            },
            totalCount = 1,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetSearchWarrantsResponse()
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    id = 98765,
                    warrantNumber = "W-2024-1234",
                    globalSubjectId = 12345,
                    subjectName = "SMITH, JOHN DAVID",
                    charge = "Failure to Appear - Traffic",
                    issueDate = "2024-03-15",
                    issuingCourt = "Metropolis Municipal Court",
                    bail = 500.00,
                    status = "Active",
                    warrantType = "Bench Warrant"
                },
                new
                {
                    id = 98766,
                    warrantNumber = "W-2023-5678",
                    globalSubjectId = 12345,
                    subjectName = "SMITH, JOHN DAVID",
                    charge = "Petty Theft",
                    issueDate = "2023-11-01",
                    issuingCourt = "Metropolis Municipal Court",
                    bail = 1000.00,
                    status = "Active",
                    warrantType = "Arrest Warrant"
                }
            },
            totalCount = 2,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetSearchAlertsResponse()
    {
        var response = new
        {
            results = new[]
            {
                new
                {
                    id = 301,
                    globalSubjectId = 12345,
                    subjectName = "SMITH, JOHN DAVID",
                    alertType = "Armed and Dangerous",
                    category = "Officer Safety",
                    description = "Subject has history of violent resistance and may be armed",
                    expirationDate = "2025-06-24",
                    isActive = true,
                    createdDate = "2024-01-15",
                    createdBy = "Sgt. Thompson"
                },
                new
                {
                    id = 302,
                    globalSubjectId = 12345,
                    subjectName = "SMITH, JOHN DAVID",
                    alertType = "Gang Affiliation",
                    category = "Intelligence",
                    description = "Known member of Northside Gang",
                    expirationDate = null as string,
                    isActive = true,
                    createdDate = "2023-08-10",
                    createdBy = "Det. Garcia"
                }
            },
            totalCount = 2,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    public static string GetPersonActivityResponse(int globalSubjectId)
    {
        var response = new
        {
            globalSubjectId,
            subjectName = "SMITH, JOHN DAVID",
            activities = new[]
            {
                new
                {
                    date = "2024-06-20T22:15:00Z",
                    type = "Arrest",
                    recordId = 50123,
                    description = "Arrested for Possession of Controlled Substance",
                    location = "500 MARKET ST, METROPOLIS, CA",
                    officer = "Officer K. Williams"
                },
                new
                {
                    date = "2024-05-10T15:30:00Z",
                    type = "Field Interview",
                    recordId = 40567,
                    description = "FI contact during suspicious person call",
                    location = "200 BROADWAY, METROPOLIS, CA",
                    officer = "Officer L. Martinez"
                },
                new
                {
                    date = "2024-03-22T11:00:00Z",
                    type = "Incident - Subject",
                    recordId = 2024000456,
                    description = "Named as suspect in Theft case",
                    location = "1500 COMMERCIAL DR, METROPOLIS, CA",
                    officer = "Det. Johnson"
                },
                new
                {
                    date = "2023-11-05T08:45:00Z",
                    type = "Citation",
                    recordId = 70123,
                    description = "Traffic citation - Speeding 75/55",
                    location = "I-5 @ MM 45, METROPOLIS, CA",
                    officer = "Officer P. Davis"
                },
                new
                {
                    date = "2023-09-14T19:20:00Z",
                    type = "Incident - Victim",
                    recordId = 2023005678,
                    description = "Victim of Assault",
                    location = "800 PARK AVE, METROPOLIS, CA",
                    officer = "Officer R. Chen"
                }
            },
            totalCount = 5,
            hasMore = false
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }
}
