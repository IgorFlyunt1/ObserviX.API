namespace ObserviX.Shared.Entities
{
    // Base entity is assumed to be defined elsewhere.
    public class Visitor : BaseEntity
    {
        public Guid VisitorId { get; set; }
        
        // Identity Information
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LeadId { get; set; }
        public string? ClientId { get; set; }
        public string? ExternalUserId { get; set; } // For logged-in users

        // Technical Information
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsReturning { get; set; }
        public string? DeviceFingerprint { get; set; } // Hash of device characteristics
        
        // Engagement Metrics
        public int TotalVisits { get; set; }
        public int TotalConversions { get; set; }
        public int TotalPageViews { get; set; }
        public DateTime FirstVisitDate { get; set; }
        public DateTime LastVisitDate { get; set; }
        public TimeSpan TotalEngagementTime { get; set; } // Accumulated time across sessions
        public GeoLocation? Location { get; set; }
        public int TotalOrganicSearches { get; set; }
        public int TotalPaidSearches { get; set; }

        // Relationships
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<VisitorConsent> VisitorConsents { get; set; } = new List<VisitorConsent>();
        public ICollection<VisitorGoal> VisitorGoals { get; set; } = new List<VisitorGoal>();
    }

    public class Session : BaseEntity
    {
        public Guid SessionId { get; set; }
        public Guid VisitorId { get; set; }
        public Visitor? Visitor { get; set; }
        
        // Timing Information
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        
        // Session Context
        public string? EntryPage { get; set; }
        public string? ExitPage { get; set; }
        public string? Referrer { get; set; }
        public string? LandingPage { get; set; }
        public string? SearchQuery { get; set; } // On-site search terms
        public bool IsOrganic { get; set; }
        
        // Technical Details
        public DeviceDetails? Device { get; set; }
        public NetworkDetails? Network { get; set; }
        public SecurityFlags? Security { get; set; }
        public GeoLocation? Location { get; set; }
        
        // Quality Metrics and Engagement Outcome
        public bool HadConversions { get; set; }
        public int InteractionCount { get; set; } // Count of interactions (clicks, hovers, etc.)
        public bool IsConverted { get; set; } // Indicates if the session led to a conversion
        public double? EngagementScore { get; set; } // Optional metric for session quality
        public SessionEndReason EndReason { get; set; }
    
        // Relationships
        public ICollection<PageView> PageViews { get; set; } = new List<PageView>();
        public ICollection<UtmParameters> UtmParameters { get; set; } = new List<UtmParameters>();
        public ICollection<SessionEvent> SessionEvents { get; set; } = new List<SessionEvent>();
        public ICollection<ClientError> ClientErrors { get; set; } = new List<ClientError>();
    }

    public class PageView
    {
        public Guid PageViewId { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        
        public string Url { get; set; } = null!;
        public string? PageTitle { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public int ScrollDepth { get; set; } // Percentage scrolled
        public int MouseActivityCount { get; set; } // Total mouse movements/clicks
        public bool HadFormInteraction { get; set; }
    }

    public class UtmParameters
    {
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        
        public string? UtmSource { get; set; }
        public string? UtmMedium { get; set; }
        public string? UtmCampaign { get; set; }
        public string? UtmTerm { get; set; }
        public string? UtmContent { get; set; }
        public string? UtmId { get; set; }
        public string? GclId { get; set; }
        public string? Fbclid { get; set; }
        
        // Custom UTM parameters as key-value pairs.
        public Dictionary<string, string>? AdditionalUtmParameters { get; set; }
        
        // Changed from string to enum (nullable) to ensure valid values.
        public MarketingChannelType? MarketingChannel { get; set; }
    }

    public class SessionEvent
    {
        public Guid EventId { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        
        // Use the EventType enum instead of a plain string.
        public EventType EventType { get; set; } = EventType.Custom;
        public string EventName { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public string? ElementId { get; set; } // UI element that triggered the event
        public string? CustomProperties { get; set; } // JSON metadata
    }

    public class ClientError
    {
        public Guid ErrorId { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        
        // Use the ErrorType enum.
        public ErrorType ErrorType { get; set; } = ErrorType.JavaScript;
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
        public string? Component { get; set; }
        public DateTime OccurredAt { get; set; }
    }

    public class GeoLocation
    {
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Timezone { get; set; }
    }

    public class VisitorGoal
    {
        public Guid GoalId { get; set; }
        public Guid VisitorId { get; set; }
        public Visitor? Visitor { get; set; }
        
        public string GoalName { get; set; } = null!;
        public DateTime AchievedAt { get; set; }
        public string? ConversionValue { get; set; }
        public string? GoalProperties { get; set; } // JSON metadata
    }

    public class DeviceDetails
    {
        // Using the DeviceType enum defined below.
        public DeviceType Type { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? OS { get; set; }
        public string? OSVersion { get; set; }
        public string? Browser { get; set; }
        public string? BrowserVersion { get; set; }
        public string? ScreenResolution { get; set; }
        public string? Timezone { get; set; }
        public string? ColorDepth { get; set; }
        public bool IsTouchEnabled { get; set; }
        public string? CPUArchitecture { get; set; }
        public int? MemorySize { get; set; } // In GB
    }

    public class NetworkDetails
    {
        public string? IpAddress { get; set; }
        public string? ASN { get; set; }
        public string? ISP { get; set; }
        public string? Organization { get; set; }
        public string? ConnectionType { get; set; } // e.g., wifi, cellular, cable
        public string? NetworkSpeed { get; set; }
        public int? ConnectionLatency { get; set; } // in milliseconds
    }

    public class SecurityFlags
    {
        public bool? IsProxy { get; set; }
        public bool? IsTor { get; set; }
        public bool? IsVPN { get; set; }
        public bool? IsMaliciousIP { get; set; }
        public bool? IsKnownAttacker { get; set; }
    }

    public class VisitorConsent
    {
        public Guid ConsentId { get; set; }
        public Guid VisitorId { get; set; }
        public Visitor? Visitor { get; set; }
        
        // Although consent type values (like GDPR, CCPA) might vary, keeping this as a string allows flexibility.
        public string ConsentType { get; set; } = "GDPR"; 
        public bool ConsentGiven { get; set; }
        public DateTime GrantedAt { get; set; }
        public string? ConsentVersion { get; set; }
        public string? Preferences { get; set; } // JSON storage for consent options
        
        // Indicates where the consent was captured, e.g., "website", "mobile app".
        public string? ConsentSource { get; set; }
    }

    // Enums

    public enum EventType
    {
        Click,
        Download,
        VideoPlay,
        FormSubmit,
        AddToCart,
        Purchase,
        Custom
    }

    public enum ErrorType
    {
        JavaScript,
        Network,
        Console,
        Performance,
        Security
    }

    public enum DeviceType
    {
        Desktop,
        Mobile,
        Tablet,
        Other
    }

    public enum MarketingChannelType
    {
        Organic,
        PaidSearch,
        SocialMedia,
        Email,
        Direct,
        Referral,
        Google,
        Facebook
    }

    public enum SessionEndReason
    {
        UserNavigation,
        Inactivity,
        BrowserClose,
        Timeout
    }
}
