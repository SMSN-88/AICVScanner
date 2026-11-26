using Microsoft.AspNetCore.Identity;


namespace AICVScanner.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CvDocument> CvDocuments { get; set; } = new List<CvDocument>();
        public ICollection<ScanResult> ScanResults { get; set; } = new List<ScanResult>();
    }
}
