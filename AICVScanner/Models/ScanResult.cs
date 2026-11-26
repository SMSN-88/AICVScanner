namespace AICVScanner.Models;

public class ScanResult
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public int CvDocumentId { get; set; }
    public CvDocument CvDocument { get; set; } = null!;
    public string JobDescription { get; set; } = string.Empty;
    public int MatchScore { get; set; }
    public int AtsScore { get; set; }
    public string MissingSkills { get; set; } = string.Empty;
    public string Suggestions { get; set; } = string.Empty;
    public string KeywordAnalysis { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}