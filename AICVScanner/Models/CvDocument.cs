namespace AICVScanner.Models
{
    public class CvDocument
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ScanResult> ScanResults { get; set; } = new List<ScanResult>();
    }
}
