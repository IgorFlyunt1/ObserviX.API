namespace ObserviX.Shared.Entities;

public class OrganicSearch
{
    public Guid SearchId { get; set; }
    public Guid SessionId { get; set; }
    public Session? Session { get; set; }

    // Search Engine Details
    public SearchEngineType Engine { get; set; }
    public string? Keyword { get; set; }
    public string? SearchQuery { get; set; }
    
    // Enhanced Tracking
    public string? Language { get; set; }
    public string? CountryCode { get; set; }
    public bool IsPaidAdClick { get; set; }
    public int PositionRank { get; set; } // Position in search results
}

public enum SearchEngineType
{
    Google,
    Bing,
    Yahoo,
    DuckDuckGo,
    Baidu,
    Yandex,
    Other
}