namespace ObserviX.Shared.Entities;

public class Attribution : BaseEntity
{
    public Guid AttributionId { get; set; }
    
    // Enhanced Conversion Tracking
    public Guid? PrimaryConversionId { get; set; }
    public ICollection<ConversionEvent> RelatedConversions { get; set; } = new List<ConversionEvent>();
    
    // Multi-Touch Configuration
    public AttributionModelType ModelType { get; set; }
    public AttributionWindow Window { get; set; } = new();
    public decimal DecayFactor { get; set; } = 0.5m; // For time-decay models
    
    // AI/ML Integration
    public string ModelVersion { get; set; } = "1.0";
    public DateTime ModelTrainingDate { get; set; }
    public string FeatureSet { get; set; } = "basic"; // advanced, custom
    
    // Financial Metrics
    // [Precision(18, 2)]
    public decimal TotalRevenue { get; set; }
    // [Precision(18, 2)]
    public decimal AttributedRevenue { get; set; }
    public int CostPerTouchpoint { get; set; }
    public decimal ROAS => CostPerTouchpoint > 0 ? AttributedRevenue / CostPerTouchpoint : 0;

    // Relationships
    public ICollection<AttributionTouchpoint> Touchpoints { get; set; } = new List<AttributionTouchpoint>();
    public ICollection<AttributionPath> ConversionPaths { get; set; } = new List<AttributionPath>();
    
    // B2B Attribution
    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
    public int DecisionMakerCount { get; set; }
    public TimeSpan SalesCycleDuration { get; set; }
    
    // Temporal Analysis
    public DateTime FirstTouch { get; set; }
    public DateTime LastTouch { get; set; }
    public TimeSpan ConversionLag { get; set; }
    
    // Model Performance
    public decimal ConfidenceScore { get; set; }
    public decimal ModelAccuracy { get; set; }
    public bool IsAnomaly { get; set; }
}

public class AttributionTouchpoint : BaseEntity
{
    public Guid TouchpointId { get; set; }
    public Guid AttributionId { get; set; }
    
    // Channel Details
    public ChannelType Channel { get; set; }
    public string SubChannel { get; set; } = null!;
    public string CampaignId { get; set; } = null!;
    
    // Engagement Metrics
    public decimal EngagementScore { get; set; }
    public int InteractionCount { get; set; }
    public TimeSpan DwellTime { get; set; }
    
    // Financials
    // [Precision(18, 2)]
    public decimal Cost { get; set; }
    public decimal Weight { get; set; } // Attribution weight 0-1
    
    // Temporal Data
    public DateTime OccurredAt { get; set; }
    public TimeSpan TimeToConversion { get; set; }
    
    // Content Context
    public string ContentType { get; set; } = "Unknown";
    public string ContentId { get; set; } = null!;
    public string? SearchKeywords { get; set; }
    
    // Privacy
    public bool IsAnonymized { get; set; }
    public DateTime? AnonymizedAt { get; set; }
}

public class AttributionPath
{
    public Guid PathId { get; set; }
    public string PathHash { get; set; } = null!; // Hash of touchpoint sequence
    
    // Path Analysis
    public int PathLength { get; set; }
    public string ChannelSequence { get; set; } = null!;
    public decimal PathEfficiency { get; set; } // 0-1 score
    
    // Markov Chain Properties
    public decimal RemovalImpact { get; set; }
    public decimal TransitionProbability { get; set; }
    
    // Conversion Impact
    public int ConversionCount { get; set; }
    // [Precision(18, 2)]
    public decimal TotalPathRevenue { get; set; }
}

public class AttributionWindow
{
    public TimeSpan LookbackWindow { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan ConversionWindow { get; set; } = TimeSpan.FromDays(7);
    public bool IncludePostConversion { get; set; } = false;
}

public enum AttributionModelType
{
    FirstTouch,
    LastTouch,
    Linear,
    TimeDecay,
    PositionBased,
    MarkovChain,
    ShapleyValue,
    SurvivalAnalysis,
    Algorithmic,
    CustomAI
}

public enum ChannelType
{
    PaidSearch,
    OrganicSearch,
    Email,
    SocialMedia,
    Direct,
    Referral,
    Affiliate,
    Partner,
    ContentSyndication,
    Webinar,
    Event,
    SalesOutreach,
    AccountBasedMarketing,
    Retargeting,
    OrganicSocial,
    NativeAdvertising
}

// Enhanced Conversion Event
public class ConversionEvent
{
    public Guid ConversionId { get; set; }
    public ConversionType Type { get; set; }
    public DateTime OccurredAt { get; set; }
    // [Precision(18, 2)]
    public decimal Value { get; set; }
    public string? ExternalId { get; set; } // CRM opportunity ID
    public string? FunnelStage { get; set; }
}

public enum ConversionType
{
    Lead,
    MQL,
    SQL,
    Opportunity,
    ClosedWon,
    Renewal,
    Upsell,
    DemoRequested,
    WhitepaperDownload
}

// Enhanced Account Model
public class Account
{
    public Guid AccountId { get; set; }
    public string? Industry { get; set; }
    public int EmployeeCount { get; set; }
    public decimal AnnualRevenue { get; set; }
    public string? TechStack { get; set; } // JSON array
    public int AccountTier { get; set; } // 1-5 based on potential
    
    // Relationships
    public ICollection<Attribution> Attributions { get; set; } = new List<Attribution>();
    public ICollection<OfflineInteraction> Interactions { get; set; } = new List<OfflineInteraction>();
}

// Advanced Offline Interaction
public class OfflineInteraction
{
    public Guid InteractionId { get; set; }
    public InteractionType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Participants { get; set; } // JSON array
    public string? MeetingNotes { get; set; } // Encrypted
    public string? RelatedCampaign { get; set; }
    public string? Outcome { get; set; }
    // [Precision(18, 2)]
    public decimal EstimatedImpact { get; set; }
}

public enum InteractionType
{
    SalesCall,
    ProductDemo,
    ConferenceMeeting,
    PartnerMeeting,
    CustomerSuccessReview,
    ExecutiveBriefing,
    TradeShow,
    WebinarAttendance
}

// Markov Chain Transition Matrix
public class TransitionMatrix
{
    public Dictionary<string, Dictionary<string, decimal>> Transitions { get; set; } = new();
    public decimal ConversionRate { get; set; }
    public DateTime CalculatedAt { get; set; }
}

// Shapley Value Implementation
public class ShapleyAttribution
{
    public Dictionary<string, decimal> ChannelValues { get; set; } = new();
    public decimal TotalValue { get; set; }
    public decimal EfficiencyScore { get; set; }
}

// // Indexes for Query Optimization
// [Index(nameof(AccountId), nameof(ConversionDate))]
// [Index(nameof(ModelType), nameof(IsAnomaly))]
// public class Attribution : BaseEntity { }
//
// [Index(nameof(Channel), nameof(OccurredAt))]
// public class AttributionTouchpoint : BaseEntity { }