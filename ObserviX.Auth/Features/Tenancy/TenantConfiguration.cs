using System.ComponentModel.DataAnnotations;

namespace ObserviX.Auth.Features.Tenancy;

public class TenantConfiguration
{
    public Guid TenantId { get; set; }
    public string? TenantName { get; set; }
    public string? Domain { get; set; }
    public int RateLimitPerMinute { get; set; }
    public int RateLimitPerHour { get; set; }
    public int RateLimitPerDay { get; set; }
    public bool IsFeatureXEnabled { get; set; }
    public bool IsFeatureYEnabled { get; set; }
    public string? AnalyticsSetting { get; set; }
    public bool IsAdvancedAnalyticsEnabled { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public string? SupportEmail { get; set; }
    public decimal MonthlyUsageQuota { get; set; }
    public decimal CurrentUsage { get; set; }
}